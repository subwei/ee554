import java.io.*;
import java.net.*;
import javax.swing.*;

public class TankClient{
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

    public TankClient() {
        // create game and registration frames
        game = new GameGUI(this);
        game.setSize(GameGUI.WIDTH, GameGUI.LENGTH);
        game.setVisible(true);
        registration = new RegisterGUI(this);
        registration.setSize(RegisterGUI.WIDTH, RegisterGUI.LENGTH);
        registration.setVisible(true);
        
        // set up socket to communicate with server
        try {
            socket = new DatagramSocket();
        }
        catch (SocketException e) {
            e.printStackTrace();
        }
        
        // listen for packets from server
        listenForTraffic();
    }
    
    private void listenForTraffic(){
        while (true){
            // put socket listening code here
            byte[] data = new byte[2];
            receivePacket = new DatagramPacket(data, data.length);
            try {
                socket.receive(receivePacket);
            }
            catch (IOException e) {
                e.printStackTrace();
            }
            
            //receivePacket.getData();
        }
    }
    
    /** Creates the registration frame if it does not
     * already exist
     */
    public void createRegistration(){
        if (!registration.isActive()){
            registration = new RegisterGUI(this);
            registration.setSize(RegisterGUI.WIDTH, RegisterGUI.LENGTH);
            registration.setVisible(true);
        }
    }
    
    /** 
     * sends packet to server with direction information
     * @param direction
     */
    public void sendPacket(int direction){
        try {
            byte[] data = new byte[2];
            data[0] = (byte)(0x00);
            data[1] = (byte)(0x30);
            // get a datagram socket
            socket = new DatagramSocket();
            netAddress = InetAddress.getByName(hostName);
            //address = InetAddress.getByName(ipAddress);
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
            data[1] = (byte)(0x30);
            sendPacket = new DatagramPacket(data, data.length, netAddress, portNumber);
            socket.send(sendPacket);
            System.out.println("Packet Sent"); 
        }
        catch (IOException ex) {
            ex.printStackTrace();
        }
        
        game.moveTank(0);
        game.moveTank(1);
        game.moveTank(2);
        game.moveTank(3);
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

    public static void main(String args[]) {
        TankClient tankClient = new TankClient();
    }
}
