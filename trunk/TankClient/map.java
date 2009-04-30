import java.awt.*;
import java.applet.*;
import java.awt.event.*;

public class map extends Applet implements KeyListener {
//Variable Declaration
   int across;
   int vert;
   int x=40;
   int y=40;
   int direction = 0;
   int width = 15;
   int height = 15;
   int[][] A = {  
		   			{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
		   			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
		   			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
		   			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
		   			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
		   			{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
		   			{1,0,0,0,0,1,1,1,1,1,1,0,0,0,1},
		   			{1,0,0,0,0,1,1,1,1,1,1,0,0,0,1},
                  	{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                  	{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                  	{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                  	{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                  	{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                  	{1,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                  	{1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
               };
 //End Variable Delcaration
   
 // Image Declaration
    Image road = Toolkit.getDefaultToolkit().getImage("road.gif");
    Image wall = Toolkit.getDefaultToolkit().getImage("brick.jpg");
    Image tank = Toolkit.getDefaultToolkit().getImage("tank.jpg");
    Image tank_left = Toolkit.getDefaultToolkit().getImage("tank_left.jpg");
    Image tank_down = Toolkit.getDefaultToolkit().getImage("tank_down.jpg");
    Image tank_right = Toolkit.getDefaultToolkit().getImage("tank_right.jpg");
 // End Image Delclaration
    
    MediaTracker track = new MediaTracker(this);
    public void init() {
       // Register images with media tracker          
        track.addImage(road, 0);
        track.addImage(wall,1);
        track.addImage(tank,3);
        track.addImage(tank_left,4);
        track.addImage(tank_down, 5);
        track.addImage(tank_right, 6);
        // populate ints    
        addKeyListener(this);
    } 

    public void paint(Graphics g) {
       
        for(int across = 0; across < width ; across++) {
            for(int vert = 0; vert < height ; vert++) {
                if (A[across][vert] == 0) {
                    // we draw road
                    g.drawImage(road,across*40,vert*40,this);
                } else {
                    // we draw wall
                    g.drawImage(wall,across*40,vert*40,this);    
                 }
            }
        }
        //draw tank
        	if (direction == 0)
        		g.drawImage(tank,x,y,this);
        	if (direction == 1)
        		g.drawImage(tank_left,x,y,this);
        	if (direction == 2)
        		g.drawImage(tank_down,x,y,this);
        	if (direction == 3)
        		g.drawImage(tank_right,x,y,this);
        	
    }
    
    public void keyPressed(KeyEvent evt)
    {    
       int key = evt.getKeyCode();
       if(key == KeyEvent.VK_LEFT)
        {
            x=x-40;
            if (A[x/40][y/40]==0)
            {
            	direction = 1;
            	repaint();           
            }
            else
            {
            	x=x+40;
            	direction = 1;
            }
        }
        if(key == KeyEvent.VK_RIGHT)
        {
            x=x+40;
            if (A[x/40][y/40]==0)
            {
            	direction = 3;
            	repaint();
            }
            else
            {
            	x=x-40;
            	direction = 3;
            }
        }
        if(key == KeyEvent.VK_UP)
        {
            y=y-40;
            if (A[x/40][y/40]==0)
            {
            	direction = 0;
            	repaint();
            }
            else
            {
            	y=y+40;
            	direction = 0;
            }
        }
        if(key == KeyEvent.VK_DOWN)
        {
            y=y+40;
            if (A[x/40][y/40]==0)
            {
            	direction = 2;
            	repaint();
            }
            else
            {
            	y=y-40;
            	direction = 2;
            }
        }
    }
    public void keyReleased(KeyEvent evt)
    {   
    }
    public void keyTyped(KeyEvent evt)
    {     
    }
}

