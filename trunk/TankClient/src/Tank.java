import java.awt.Color;

import javax.swing.JLabel;

public class Tank extends JLabel {

    private int health;
    private int x;
    private int y;

    public static final int SIZE = 50;
    public static final int MOVE = 10;

    public Tank(String name) {

        x = GameGUI.WIDTH / 2;
        y = GameGUI.LENGTH / 2;
        setLocation(0, 0);

        setText(name);
        setForeground(Color.WHITE);
        setSize(SIZE, SIZE);
    }
    
    public void moveTo(int x, int y){
        this.x = x;
        this.y = y;
        setLocation(x,y);
    }

    public int getX() {
        return x;
    }

    public int getY() {
        return y;
    }
    
    public void moveUp() {
        if (y >= SIZE) {
            y -= MOVE;
            setLocation(x, y);
        }
    }

    public void moveDown() {
        if (y <= GameGUI.LENGTH - SIZE) {
            y += MOVE;
            setLocation(x, y);
        }
    }

    public void moveRight() {
        if (x <= GameGUI.WIDTH - SIZE) {
            x += MOVE;
            setLocation(x, y);
        }
    }

    public void moveLeft() {
        if (x >= SIZE) {
            x -= MOVE;
            setLocation(x, y);
        }
    }

}
