import java.awt.Color;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.util.ArrayList;

import javax.swing.ImageIcon;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;

public class GameGUI extends JFrame implements KeyListener, ActionListener{
    // Width and Length of the game frame
    public static final int WIDTH   = 800;
    public static final int LENGTH  = 600;
    
    // directions in the game
    public static final byte NORTH      = (byte)0x01;
    public static final byte WEST       = (byte)0x02;
    public static final byte SOUTH      = (byte)0x04;
    public static final byte EAST       = (byte)0x08;
    public static final byte NORTHWEST  = (byte)0x10;
    public static final byte NORTHEAST  = (byte)0x20;
    public static final byte SOUTHWEST  = (byte)0x40;
    public static final byte SOUTHEAST  = (byte)0x80;
    
    // Image Icons for the tank
    private ImageIcon northImage;
    private ImageIcon southImage;
    private ImageIcon eastImage;
    private ImageIcon westImage;
    
    // Menu Components
    private JMenuBar menuBar;
    private JMenu menu;
    private JMenuItem register;
    
    private ArrayList<Tank> tanks;
    private TankClient tankClient;
    private Tank tank;
    private byte orientation;
    private ArrayList<JLabel> bullets;
    
    public GameGUI(TankClient tankClient){
        this.tankClient = tankClient;
        setBackground(Color.BLACK);
        tanks = new ArrayList<Tank>();
        bullets = new ArrayList<JLabel>();
        for (int x=0; x<4; x++){
            JLabel bullet = new JLabel("0");
            bullet.setSize(8, 8);
            bullet.setForeground(Color.WHITE);
            bullet.setVisible(false);
            add(bullet);
            bullets.add(bullet);
        }

        northImage = new ImageIcon("tank.jpg");
        southImage = new ImageIcon("tank_down.jpg");
        westImage = new ImageIcon("tank_left.jpg");
        eastImage = new ImageIcon("tank_right.jpg");
        setLayout(null);
        addKeyListener(this); 
        setupMenu();
    }
    
    private void setupMenu() {
        menuBar = new JMenuBar();
        menu = new JMenu("File");
        register = new JMenuItem("Register");
        register.addActionListener(this);
        menu.add(register);
        menuBar.add(menu);
        setJMenuBar(menuBar);
    }
    
    public void reset(){
        tanks.clear();
    }
    
    public void addTank(int x, int y, byte orientation){
        if (orientation == NORTH){
            tank = new Tank(northImage);
        }
        else if (orientation == SOUTH){
            tank = new Tank(southImage);
        }
        else if (orientation == EAST){
            tank = new Tank(eastImage);
        }
        else if (orientation == WEST){
            tank = new Tank(westImage);
        }
        else{
            System.out.println("Error creating Tank: invalid orientation");
        }
        tanks.add(tank);
        tank.moveTo(x, y);
        add(tank);
    }
    
    public void drawTank(int tankNum, int x, int y, byte orientation){
        if (tankNum == tankClient.getClientID()){
            this.orientation = orientation;
        }
        if(tankNum >= tanks.size()) {
        	System.out.println("Index is larger than the number of registered tanks???");
        	System.out.println("Tanks Registered: "+tanks.size());
        	System.out.println("Tank Index: "+tankNum);
        	return;
        }

        switch(orientation) {
        case NORTH:
//        	System.out.println("North orientation");
        	tanks.get(tankNum).setIcon(northImage);
        	break;
        case SOUTH:
//        	System.out.println("South orientation");
        	tanks.get(tankNum).setIcon(southImage);
        	break;
        case EAST:
//        	System.out.println("East orientation");
        	tanks.get(tankNum).setIcon(eastImage);
        	break;
        case WEST:
//        	System.out.println("West orientation");
        	tanks.get(tankNum).setIcon(westImage);
        	break;
        default:
        	System.out.println("Unk orientation");
        	break;
        }
        tanks.get(tankNum).moveTo(x, y);
        invalidate();
        repaint();
        Point p = tanks.get(tankNum).getLocation();
        System.out.println("Point: x " + p.x + " y " + p.y);
        System.out.println("Draw Tank " + tankNum + ": x " + x + " y " + y + " orient: " + (int)orientation);
    }
    
    public void drawBullet(int bulletIndex, int x, int y){
        bullets.get(bulletIndex).setVisible(true);
//        System.out.println("x: " + x + " y: " + y);
        bullets.get(bulletIndex).setLocation(x, y);
    }

    public void keyReleased(KeyEvent e) {
        if (e.getKeyCode() == KeyEvent.VK_UP){
            tankClient.sendDirectionPacket(NORTH);
            //tank.moveUp();
        }
        else if (e.getKeyCode() == KeyEvent.VK_DOWN){
            tankClient.sendDirectionPacket(SOUTH);
            //tank.moveDown();
        }
        else if (e.getKeyCode() == KeyEvent.VK_RIGHT){
            tankClient.sendDirectionPacket(EAST);
            //tank.moveRight();
        }
        else if (e.getKeyCode() == KeyEvent.VK_LEFT){
            tankClient.sendDirectionPacket(WEST);
            //tank.moveLeft();
        }
        else if (e.getKeyCode() == KeyEvent.VK_ENTER){
            tankClient.startGame();
            System.out.println("Starting Game...");
        }
        else if (e.getKeyCode() == KeyEvent.VK_SPACE){
            tankClient.shoot(orientation);
        }
        else if (e.getKeyCode() == KeyEvent.VK_Q){
            tankClient.quit();
        }
        invalidate();
        repaint();
    }

    public void actionPerformed(ActionEvent e) {
        if (e.getSource() == register){
            tankClient.createRegistration();
        }
    }

    public void keyPressed(KeyEvent e) {

    }
    
    public void keyTyped(KeyEvent e) {
        // TODO Auto-generated method stub
        
    }   
}




