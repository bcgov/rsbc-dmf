package com.gov.rsi.dmft.pdf;

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
 * Wraps calls to the Jasper Reports JasperFillManager and JasperExportManager classes 
 */
public class ReportGenerator implements Serializable{

	/**
	 * Generates a report in PDF format
	 * @param dataSource the source data for the report
	 * @param parameters report parameters 
	 * @param is source stream for the .jasper template
	 * @param os destination stream for the generated PDF document
	 * @throws JRException if an error occurs within Jasper Reports during generation of the PDF
	 * @throws IOException if either the template cannot be read, or the PDF cannot be output
	 */
	public void generate(JRDataSource dataSource, Map<String, Object> parameters, InputStream is, OutputStream os) throws JRException, IOException{

		JasperPrint jrPrint = JasperFillManager.fillReport(is, parameters, dataSource);			
		JasperExportManager.exportReportToPdfStream(jrPrint, os);
	}
}
