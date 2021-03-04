package GUI;

import javax.swing.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;


public class Login {


    private int counter = 0;

    private JButton buttonMain;
    private JPanel panel;
    private JTextArea textPaneMain;

    void Login() {
        textPaneMain.setText("0");

        buttonMain.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                textPaneMain.setText(Integer.toString(++counter));
            }
        });
    }

    public static void main(String[] args) {
        JFrame frame = new JFrame("App");
        frame.setContentPane(new Login().panel);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.pack();
        frame.setVisible(true);
        frame.setSize(500, 500);
        frame.setResizable(true);
        frame.setLocationRelativeTo(null);
    }
}
