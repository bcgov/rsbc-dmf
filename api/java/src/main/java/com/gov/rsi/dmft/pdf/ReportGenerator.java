package com.gov.rsi.dmft.pdf;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.Serializable;
import java.util.Map;

import net.sf.jasperreports.engine.JRDataSource;
import net.sf.jasperreports.engine.JRException;
import net.sf.jasperreports.engine.JasperExportManager;
import net.sf.jasperreports.engine.JasperFillManager;
import net.sf.jasperreports.engine.JasperPrint;

/**
 * The class which generates the pdf document 
 */
public class ReportGenerator implements Serializable{

	private static final String PATH = System.getProperty("user.dir");
	/**
	 * Generates a report and places it in the HttpServletResponse as application/pdf body
	 * @param jasperFilename
	*/ 
	public void generate(JRDataSource dataSource, String jasperFilename, Map<String, Object> parameters) throws JRException, IOException{
		String path = "report.pdf";
		OutputStream os = new FileOutputStream(new File(path));

		
		InputStream reportStream = new FileInputStream(jasperFilename);
		JasperPrint jrPrint = JasperFillManager.fillReport(reportStream, parameters, dataSource);			
		JasperExportManager.exportReportToPdfStream(jrPrint, os);
	}


	/**
	 * Generates a report and saves it as a file. This method can run outside the Servlet context
	 * @param jasperFilename
	 */
	public void generate(JRDataSource dataSource, Map<String, Object> parameters, String jasperFilename, String reportFilename) throws JRException, IOException{
		File file = new File(reportFilename);
		OutputStream os = new FileOutputStream(createFile(reportFilename));

		InputStream reportStream = new FileInputStream(createFile(jasperFilename));
		JasperPrint jrPrint = JasperFillManager.fillReport(reportStream, parameters, dataSource);			
		JasperExportManager.exportReportToPdfStream(jrPrint, os);
	}
	
	/**
	 * Generates a report and saves it as streamt
	 * @param jasperFilename
	 */
	public void generate(JRDataSource dataSource, Map<String, Object> parameters, InputStream is, OutputStream os) throws JRException, IOException{

		JasperPrint jrPrint = JasperFillManager.fillReport(is, parameters, dataSource);			
		JasperExportManager.exportReportToPdfStream(jrPrint, os);
	}
	
	private File createFile(String name) {
		return new File(PATH + "\\" + name);
	}

}
