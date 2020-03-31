package com.gov.rsi.dmft.pdf;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.nio.file.Files;
import java.nio.file.attribute.FileTime;

import net.sf.jasperreports.engine.JRException;
import net.sf.jasperreports.engine.JasperCompileManager;
import net.sf.jasperreports.engine.data.JRBeanArrayDataSource;

/**
 * Base class for all Jasper Reports backer classes  
 */
public abstract class Backer{
	
	public Backer(){}
	
	/**
	 * Wraps an object of this class as a JasperReports JRBeanArrayDataSource object
	 * @return the JRBeanArrayDataSource
	 */
	public JRBeanArrayDataSource asDataSource(){
		JRBeanArrayDataSource dataSource;
		Backer[] backer = new Backer[1];
		backer[0] = this;
		dataSource = new JRBeanArrayDataSource(backer);
		return dataSource;
	}
	

	/**
	 * Compiles the jrxml file into a jasper file in the resources folder folder
	 * @param reportName the name of the .jrxml and .jasper files
	 */
	protected void compile(String reportName){
		try {
			
//			URL u1 = this.getClass().getClassLoader().getResource("dmer.jasper");
//			String jasperPath = u1.getFile();
			
			
			String projectPath = (new File("./")).getAbsolutePath();
			projectPath = projectPath.substring(0, projectPath.lastIndexOf('.'));
			
			String jrxmlPath = projectPath + "reports\\" + reportName + ".jrxml";
			String jasperPath = projectPath + "reports\\" + reportName + ".jasper";
			
			File jrxmlFile = new File(jrxmlPath);
			File jasperFile = new File(jasperPath);
			
			if (jasperFile.exists()) {
				FileTime jrxmlTime = Files.getLastModifiedTime(jrxmlFile.toPath());
				FileTime jasperTime = Files.getLastModifiedTime(jasperFile.toPath());
				if (jasperTime.compareTo(jrxmlTime) > 0) {
					System.out.println("Up to date Jasper File already exists");
					return;
				}
			}

			JasperCompileManager.compileReportToFile(jrxmlPath, jasperPath);
			System.out.println(reportName + " compiled successfully");
		}
		catch (JRException | IOException e){
			System.out.println(e);
		}
	}
}
