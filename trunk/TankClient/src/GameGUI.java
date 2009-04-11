import java.awt.Color;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;

import javax.swing.JFrame;
import javax.swing.JMenu;
import javax.swing.JMenuBar;
import javax.swing.JMenuItem;

public class GameGUI extends JFrame implements KeyListener, ActionListener{
    // Width and Length of the game frame
    public static final int WIDTH = 800;
    public static final int LENGTH = 600;
    // Menu Components
    private JMenuBar menuBar;
    private JMenu menu;
    private JMenuItem register;
    
    private Tank tank;              // Tank JLabel
    private TankClient tankClient;  // reference to the tankClient
    
    public GameGUI(TankClient tankClient){
        this.tankClient = tankClient;
        setBackground(Color.BLACK);
        tank = new Tank("Tank");
        add(tank);
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
    
    public void moveTank(int direction){
        if (direction == 0){
            tank.moveLeft();
        }
        else if (direction == 1){
            tank.moveRight();
        }
        else if (direction == 2){
            tank.moveDown();
        }
        else if (direction == 3){
            tank.moveUp();
        }
    }

    public void keyReleased(KeyEvent e) {
        if (e.getKeyCode() == KeyEvent.VK_UP){
            tankClient.sendPacket(0);
            //tank.moveUp();
        }
        else if (e.getKeyCode() == KeyEvent.VK_DOWN){
            tankClient.sendPacket(0);
            //tank.moveDown();
        }
        else if (e.getKeyCode() == KeyEvent.VK_RIGHT){
            tankClient.sendPacket(0);
            //tank.moveRight();
        }
        else if (e.getKeyCode() == KeyEvent.VK_LEFT){
            tankClient.sendPacket(0);
            //tank.moveLeft();
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
