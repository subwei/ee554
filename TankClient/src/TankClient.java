import java.awt.Color;
import java.awt.Dimension;
import java.io.*;
import java.net.*;
import javax.swing.*;

public class TankClient{
    
    //  constants for direction parameter 
    private static final byte MSG_MOVE      = (byte)0x10;
    private static final byte MSG_SHOOT     = (byte)0x20;
    private static final byte MSG_REGISTER  = (byte)0x30;
    private static final byte MSG_QUIT      = (byte)0x40;
    private static final byte MSG_DEAD      = (byte)0x50;
    private static final byte MSG_JOIN      = (byte)0x60;
    private static final byte MSG_START     = (byte)0x70;
    private static final byte MSG_BROADCAST = (byte)0xFF;
    private RegisterGUI registration;   // JFrame where client registers with the the tank server
    private GameGUI game;               // JFrame where the game is played
    
    // Network sockets and packets
    private DatagramSocket socket;          // Datagram socket for sending/receiving packets to/from server
    private DatagramPacket sendPacket;      // The packet to be sent to the server
    private DatagramPacket receivePacket;   // The packet received from the server
    
    // Server Information
    private String hostName;
    private String ipAddress;
    private int portNumber;
    private InetAddress netAddress;     // Server InetAddress
    private byte clientID; 
    private boolean isFirstPacket;
    
    public TankClient() {
        
        // create registration frame
        registration = new RegisterGUI(this);
        registration.setSize(RegisterGUI.WIDTH, RegisterGUI.LENGTH);
        registration.setVisible(true);
        isFirstPacket = true;
        game = null;
        clientID = -1;
        
        // set up socket and listen for traffic
        try {
            socket = new DatagramSocket();
            listenForTraffic();
        }
        catch (SocketException e) {
            e.printStackTrace();
        }
    }
    
    public void setFirst() {
    	this.isFirstPacket = true;
    }
    
    private void listenForTraffic(){
        while (true){
            byte[] data = new byte[64];
            receivePacket = new DatagramPacket(data, data.length);
            try {
                socket.receive(receivePacket);
                
                if (receivePacket.getLength() == 1){    // registration packet
                    setupGame(receivePacket.getData());
                }
                else if (receivePacket.getData()[0] == -1){
                    refreshPlayers(receivePacket);
                }
                else{
                    System.out.println("Unknown Packet Received");
                    System.out.println(receivePacket.getLength());
                    for (int x=0; x<receivePacket.getLength(); x++){
                        System.out.print(receivePacket.getData()[x] + " ");
                        //System.out.print(Integer.toHexString(((int)receivePacket.getData()[x]))  + " ");
                    }
                }
            }
            catch (IOException e) {
                e.printStackTrace();
            }
        }
    }
    
    /**
     * Create the game frame
     * @param data
     */
    private void setupGame(byte[] data){
        System.out.println("Creating Game");
        clientID = data[0];
        if (game == null){
            game = new GameGUI(this);
            game.setSize(new Dimension(GameGUI.WIDTH, GameGUI.LENGTH));
            game.setVisible(true);
        }
        else{
            game.reset();
        }
        
        // close registration screen
        registration.dispose();
        
        // Display a dialog stating which player this is
        JOptionPane.showMessageDialog(game,"You have successfully registered as player "+(clientID+1));
    }
    
    /**
     * Update the game screen with the position of the active players
     * @param data
     */
    private void refreshPlayers(DatagramPacket receivePacket){
        
//    	System.out.println("Pkt Size: "+receivePacket.getLength());
//        for (int x=0; x<receivePacket.getLength(); x++){
//            System.out.print(receivePacket.getData()[x] + " ");
//        }
        int numPlayers = (int)receivePacket.getData()[1];
        
        if (isFirstPacket){
            initializePlayers(receivePacket);
            isFirstPacket = false;
        }
        
        int numBullets = (int)receivePacket.getData()[2];
        int m = 0;
        for (int i=3; i < (numPlayers*10) + 3; i++){
            m = (int)receivePacket.getData()[i++];
            int x = ((int)receivePacket.getData()[i++]<<24)&0xFF000000 |
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i++])&0xFF;
            int y = ((int)receivePacket.getData()[i++]<<24)&0xFF000000|
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i++])&0xFF;
            byte orientation = receivePacket.getData()[i];
//            System.out.println("\n"+orientation);
            game.drawTank(m, x, y, orientation);
        }
        if(numPlayers == 1) {
        	JOptionPane.showMessageDialog(game,"You are player "+(clientID+1)+"\n"+
        									   "Player "+(m+1)+" has won the game!");
        	return;
        }
        m = 0;
        if((numBullets*9) + (numPlayers*10) + 3 != receivePacket.getLength()) return;
        for (int i=(numPlayers*10) + 3; i< (numBullets*9) + (numPlayers*10) + 3; i++){
            m = (int)receivePacket.getData()[i++];
            int x = ((int)receivePacket.getData()[i++]<<24)&0xFF000000 |
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i++])&0xFF;
            int y = ((int)receivePacket.getData()[i++]<<24)&0xFF000000|
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i])&0xFF;
            game.drawBullet(m, x, y);
        }
    }
    
    public void initializePlayers(DatagramPacket receivePacket){

        int numPlayers = (int)receivePacket.getData()[1];        
        int m = 0;
        for (int i=3; i < (numPlayers*10) + 3; i++){
            m = (int)receivePacket.getData()[i++];
            int x = ((int)receivePacket.getData()[i++]<<24)&0xFF000000 |
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i++])&0xFF;
            int y = ((int)receivePacket.getData()[i++]<<24)&0xFF000000|
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i++])&0xFF;
            byte orientation = receivePacket.getData()[i];
            game.addTank(x, y, orientation);
            //game.drawTank(m, x, y);
        }
    }
    
    /**
     * Creates the registration frame if it doesn't already exist
     */
    public void createRegistration(){
        if (!registration.isActive()){
            registration = new RegisterGUI(this);
            registration.setSize(RegisterGUI.WIDTH, RegisterGUI.LENGTH);
            registration.setVisible(true);
        }
    }
    
    /**
     * Converts IP address string to an InetAddress
     * @param ipAddress
     * @return InetAddress
     */
    public InetAddress convertToInet(String ipAddress){
        InetAddress address = null;
        try {
            address = InetAddress.getByName(ipAddress);
        }
        catch (UnknownHostException e) {
            e.printStackTrace();
        }
        return address;
    }
    
    
    /*------------------------------------------------------
     * Methods to send information to server
     *----------------------------------------------------*/
    
    /**
     * Sends startgame packet to the server
     */
    public void startGame(){
        try {
            isFirstPacket = true;
            if(clientID == 0) {
            	byte[] data = new byte[2];
            	data[0] = (byte)(clientID);
            	data[1] = MSG_START;
            	sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            	socket.send(sendPacket);
            } else {
            	JOptionPane.showMessageDialog(game,"You are player "+(clientID+1)+" and are not authorized to start the game!");
            }
//            System.out.println("Packet Sent"); 
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
    }
   
    /** 
     * sends packet to server with direction information
     * @param direction
     */
    public void sendDirectionPacket(byte direction){
        
        try {
            byte[] data = new byte[3];
            data[0] = (byte)(clientID);
            data[1] = MSG_MOVE;
            data[2] = direction;
            sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            socket.send(sendPacket);
//            System.out.println("Packet Sent"); 
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    
    /**
     * Register with the server
     */
    public void register(){
        try {
            byte[] data = new byte[2];
            data[0] = (byte)(0x00);
            data[1] = MSG_REGISTER;
            sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            socket.send(sendPacket);
//            System.out.println("Packet Sent"); 
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    
    public void shoot(byte direction){
        try {
            byte[] data = new byte[3];
            data[0] = clientID;
            data[1] = MSG_SHOOT;
            data[2] = direction;
            sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            socket.send(sendPacket);
//            System.out.println("Packet Sent"); 
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    
    public void quit(){
        try {
            byte[] data = new byte[2];
            data[0] = clientID;
            data[1] = MSG_QUIT;
            sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            socket.send(sendPacket);
//            System.out.println("Packet Sent"); 
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    

    /*------------------------------------------------------
     * Accessor and Mutator Methods
     *----------------------------------------------------*/
    
    public void setHostName(String hostName){
        this.hostName = hostName;
    }
    
    public void setIPAddress(String ipAddress){
        this.ipAddress = ipAddress;
        netAddress = convertToInet(ipAddress);
    }
    
    public void setPortNumber(int portNumber){
        this.portNumber = portNumber;
    }
    
    public String getHostName(){
        return hostName;
    }
    
    public String getIPAddress(){
        return ipAddress;
    }

    public int getPortNumber(){
        return portNumber;
    }
    
    public int getClientID(){
        return (int)clientID;
    }

    public static void main(String args[]) {
        TankClient tankClient = new TankClient();
    }
}
