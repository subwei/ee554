import java.awt.Color;

import javax.swing.JLabel;


public class Tank extends JLabel {

	private int health;
	private int x;
	private int y;
	
	public static final int SIZE = 50;
	public static final int MOVE = 10;
	
	public Tank(String name){
		
		x = TankGUI.WIDTH/2;
		y = TankGUI.LENGTH/2;
		setLocation(x,y);

		setText(name);
		setForeground(Color.WHITE);
		setSize(SIZE,SIZE);
	}
	

	public int getX(){
		return x;
	}
	
	public int getY(){
		return y;
	}
	
	public void moveUp(){
		if (y>=SIZE){
			y-=MOVE;
			setLocation(x,y);

		}	
		
	}
	
	public void moveDown(){
		if (y<=TankGUI.LENGTH - SIZE){
			y+=MOVE;
			setLocation(x,y);

		}
	}
	
	public void moveRight(){
		if (x<=TankGUI.WIDTH - SIZE){
			x+=MOVE;
			setLocation(x,y);

		}
	}
	
	public void moveLeft(){
		if (x>= SIZE){
			x-=MOVE;
			setLocation(x,y);

		}
	}
	
}
