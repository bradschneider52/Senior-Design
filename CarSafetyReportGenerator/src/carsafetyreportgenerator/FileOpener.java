/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package carsafetyreportgenerator;

import java.io.File;
import java.io.IOException;

/**
 *
 * @author Ian
 */
public class FileOpener {
    File selectedFile1;
    File selectedFile2;
    
    public void getFile() throws IOException{
        Process p = new ProcessBuilder("explorer.exe", "/select,C:\\directory\\selectedFile").start();
        //selectedFile1 = Runtime.getRuntime().exec("explorer.exe /select," + path);
    }
}
