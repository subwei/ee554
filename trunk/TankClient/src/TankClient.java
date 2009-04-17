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
    
    public TankClient() {
        // create registration frame
        registration = new RegisterGUI(this);
        registration.setSize(RegisterGUI.WIDTH, RegisterGUI.LENGTH);
        registration.setVisible(true);
        
        // set up socket and listen for traffic
        try {
            socket = new DatagramSocket();
            listenForTraffic();
        }
        catch (SocketException e) {
            e.printStackTrace();
        }
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
        game = new GameGUI(this, 4);
        game.setSize(new Dimension(GameGUI.WIDTH, GameGUI.LENGTH));
        game.setVisible(true);
        
        // close registration screen
        registration.dispose();
    }
    
    /**
     * Update the game screen with the position of the active players
     * @param data
     */
    private void refreshPlayers(DatagramPacket receivePacket){
        
        for (int x=0; x<receivePacket.getLength(); x++){
            System.out.print(receivePacket.getData()[x] + " ");
        }
        int numPlayers = (int)receivePacket.getData()[1];
        int m = 0;
        for (int i=2; i<receivePacket.getLength(); i++){
            int x = ((int)receivePacket.getData()[i++]<<24)&0xFF000000 |
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i++])&0xFF;
            int y = ((int)receivePacket.getData()[i++]<<24)&0xFF000000|
                    ((int)receivePacket.getData()[i++]<<16)&0xFF0000 |
                    ((int)receivePacket.getData()[i++]<<8)&0xFF00 |
                    ((int)receivePacket.getData()[i])&0xFF;
            game.drawTank(m++, x, y);
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
            byte[] data = new byte[2];
            data[0] = (byte)(clientID);
            data[1] = MSG_START;
            sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            socket.send(sendPacket);
            System.out.println("Packet Sent"); 
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
            System.out.println("Packet Sent"); 
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
            System.out.println("Packet Sent"); 
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
