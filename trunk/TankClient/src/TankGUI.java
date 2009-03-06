import java.awt.Color;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;

import javax.swing.JFrame;


public class TankGUI extends JFrame implements KeyListener{

	private Tank tank;
	
	public static final int WIDTH = 800;
	public static final int LENGTH = 600;
	
	
	public TankGUI(){
		
		setBackground(Color.BLACK);
		tank = new Tank("Tank");
		
		add(tank);
		setLayout(null);
		addKeyListener(this);
		
		
		
	}
	
	public void keyTyped(KeyEvent e){
//		if (e.getKeyCode() == KeyEvent.VK_UP){
//			tank.moveUp();
//		}
//		else if (e.getKeyCode() == KeyEvent.VK_DOWN){
//			tank.moveDown();
//		}
//		else if (e.getKeyCode() == KeyEvent.VK_RIGHT){
//			tank.moveRight();
//		}
//		else if (e.getKeyCode() == KeyEvent.VK_LEFT){
//			tank.moveLeft();
//		}
//		invalidate();
//		repaint();
//		System.out.println("Key Typed");
	}

	public void keyPressed(KeyEvent e) {
		// TODO Auto-generated method stub
		if (e.getKeyCode() == KeyEvent.VK_UP){
			tank.moveUp();
		}
		else if (e.getKeyCode() == KeyEvent.VK_DOWN){
			tank.moveDown();
		}
		else if (e.getKeyCode() == KeyEvent.VK_RIGHT){
			tank.moveRight();
		}
		else if (e.getKeyCode() == KeyEvent.VK_LEFT){
			tank.moveLeft();
		}
		invalidate();
		repaint();
	}

	public void keyReleased(KeyEvent e) {
		// TODO Auto-generated method stub
		if (e.getKeyCode() == KeyEvent.VK_UP){
			tank.moveUp();
		}
		else if (e.getKeyCode() == KeyEvent.VK_DOWN){
			tank.moveDown();
		}
		else if (e.getKeyCode() == KeyEvent.VK_RIGHT){
			tank.moveRight();
		}
		else if (e.getKeyCode() == KeyEvent.VK_LEFT){
			tank.moveLeft();
		}
		invalidate();
		repaint();
		System.out.println("Key Released");
	}	
	
	public static void main(String args[]){
		
		TankGUI gui = new TankGUI();
		gui.setVisible(true);
		gui.setSize(WIDTH,LENGTH);
		
		
	}


}
