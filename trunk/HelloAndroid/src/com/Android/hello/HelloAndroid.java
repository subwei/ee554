package com.Android.hello;

import android.app.Activity;
import android.os.Bundle;
import android.widget.TextView;


public class HelloAndroid extends Activity {
    /** Called when the activity is first created. */
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        TextView tv = new TextView(this);
        tv.setText("Hello! This is the Android Emulator =)");
        setContentView(tv);
        System.out.println("Hello");
       
    }
}