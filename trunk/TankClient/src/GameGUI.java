import java.awt.Color;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.util.ArrayList;

import javax.swing.JFrame;
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
    
    // Menu Components
    private JMenuBar menuBar;
    private JMenu menu;
    private JMenuItem register;
    
    private ArrayList<Tank> tanks;
    private TankClient tankClient;
    
    public GameGUI(TankClient tankClient, int numPlayers){
        this.tankClient = tankClient;
        setBackground(Color.BLACK);
        tanks = new ArrayList<Tank>();
        for (int x=0; x<numPlayers; x++){
            tanks.add(new Tank("Tank" + Integer.toString(x)));
            add(tanks.get(x));
        }
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
    
    public void drawTank(int tankNum, int x, int y){
        //tanks.get(tankNum).moveRight();
        //tanks.get(tankNum).setLocation(x, y);
        tanks.get(tankNum).moveTo(x, y);
        invalidate();
        repaint();
        Point p = tanks.get(tankNum).getLocation();
        System.out.println("Point: x " + p.x + " y " + p.y);
        System.out.println("Draw Tank " + tankNum + ": x " + x + " y " + y);
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
        else if (e.getKeyCode() == KeyEvent.VK_SPACE){
            tankClient.startGame();
            System.out.println("Starting Game...");
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
