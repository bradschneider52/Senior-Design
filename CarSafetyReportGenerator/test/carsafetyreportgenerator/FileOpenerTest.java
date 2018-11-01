/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package carsafetyreportgenerator;

import java.io.File;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 *
 * @author Ian
 */
public class FileOpenerTest {
    FileOpener fo;
    //fo.getFile();
    public FileOpenerTest() {
        
    }
    
    @Test
    public void testFileExists(){
        File file = ;
        String fileString = file.toString();
        assert(fileString.endsWith(".dbc"));
    }
    
}
