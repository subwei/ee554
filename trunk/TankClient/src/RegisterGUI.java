import java.awt.*;
import java.awt.event.*;
import javax.swing.*;

public class RegisterGUI extends JFrame implements ActionListener{
    // Width and Length of the registration frame
    public static final int WIDTH = 400;
    public static final int LENGTH = 300;
    // Labels to describe the TextFields
    private JLabel hostLabel;
    private JLabel ipLabel;
    private JLabel portLabel;
    private JLabel statusMessage;   // prompts the user and provides feedback
    // TextFields for datagram socket information
    private JTextField hostField;   // server host name field
    private JTextField ipField;     // ip address field
    private JTextField portField;   // port number field
    
    private JButton registerButton;     // connects the client to the server
    private TankClient tankClient;      // reference to the tankClient
    
    private map mapObject;

    public RegisterGUI(TankClient tankClient){
        this.tankClient = tankClient;
        setComponents();
        setLayout();
        mapObject = new map();
    }
    
    private void setComponents(){
        hostLabel = new JLabel("Enter Host Name:");
        ipLabel = new JLabel("Enter IP Address:");
        portLabel = new JLabel("Enter Port Number:");
        hostField = new JTextField(10);
        ipField = new JTextField(10);
        portField = new JTextField(10);
        statusMessage = new JLabel();
        registerButton = new JButton("Register");
        registerButton.addActionListener(this);
        
        ipField.setText("76.89.213.52");
        portField.setText("4801");
    }
    
    private void setLayout(){
        setLayout(new GridBagLayout());
        GridBagConstraints c = new GridBagConstraints();
        // adding labels
        c.gridx = 0; c.gridy = 0; c.gridwidth = 2; c.insets = new Insets(10,10,10,10);
        add(statusMessage, c);     
        c.gridx = 0; c.gridy = 1; c.gridwidth = 1;
        add(hostLabel, c);
        c.gridx = 0; c.gridy = 2;
        add(ipLabel, c);
        c.gridx = 0; c.gridy = 3;
        add(portLabel, c);
        // adding textfields
        c.gridx = 1; c.gridy = 1;
        add(hostField, c);
        c.gridx = 1; c.gridy = 2;
        add(ipField, c);
        c.gridx = 1; c.gridy = 3;
        add(portField, c);
        // adding button
        c.gridx = 0; c.gridy = 4;
        add(registerButton, c);
    }
    
    private void eraseFields(){
        hostField.setText("");
        ipField.setText("");
        portField.setText("");
    }

    public void actionPerformed(ActionEvent e) {
        if (e.getSource() == registerButton){
            
            try{
                String hostName = hostField.getText();
                String ipAddress = ipField.getText();
                int portNumber = Integer.parseInt(portField.getText());
                eraseFields();
                if (ipAddress.length() != 0){
                    tankClient.setHostName(hostName);
                    tankClient.setIPAddress(ipAddress);
                    tankClient.setPortNumber(portNumber);
                    tankClient.register();
                }
                else{
                    statusMessage.setText("Please fill in the ipAddress field");
                }
            }
            catch (NullPointerException npe){
                statusMessage.setText("Please fill in all fields");
            }
            catch (NumberFormatException nfe){
                statusMessage.setText("Port number must be a number.  Please Try Again");
            }
            
        }
    }

}
