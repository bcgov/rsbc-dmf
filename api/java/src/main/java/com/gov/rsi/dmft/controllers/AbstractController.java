package com.gov.rsi.dmft.controllers;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

/**
 * Base class for all Spring controller classes, providing common methods
 * for response creation
 */
public abstract class AbstractController {

    /**
     * Generates an error response (400 Bad Request) 
     * @return  the ResponseEntity
     */
    protected ResponseEntity responseBadRequest(){
    	return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
     }
	
    /**
     * Generates an error response (400 Bad Request) containing 
     * an explanatory message
     * @param message the message
     * @return  the ResponseEntity
     */
    protected ResponseEntity responseBadRequest(String message){
    	return new ResponseEntity<>(message, HttpStatus.BAD_REQUEST);
     }
	
    /**
     * Generates a normal return (200 Ok) without a body
     * @return  the ResponseEntity
     */
    protected ResponseEntity responseOkNoBody(){
    	return new ResponseEntity<>(HttpStatus.OK);
     }
    
    /**
     * Generates a normal return (200 Ok) with a body
     * @param body object to be placed in the response body
     * @return  the Response
     */
    protected ResponseEntity<?> responseOkWithBody(Object body){
    	return new ResponseEntity<>(body, HttpStatus.OK);
     }
    
    /**
     * Creates a 201 Created response with no body 
     * @return the ResponseEntity
     */
    protected ResponseEntity responseCreated(){
    	return new ResponseEntity<>(HttpStatus.CREATED);
    }

    /**
     * Creates a 204 No Content response with no body 
     * @return the ResponseEntity
     */
    protected ResponseEntity responseNoContent(){
    	return new ResponseEntity<>(HttpStatus.NO_CONTENT);
    }

    /**
     * Creates a 404 Not found response with no body 
     * @return the ResponseEntity
     */
    protected ResponseEntity responseNotFound(){
    	return new ResponseEntity<>(HttpStatus.NOT_FOUND);
    }

    /**
     * Creates a 500 Server Error response with no body 
     * @return the ResponseEntity
     */
    protected ResponseEntity responseServerError(){
    	return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
    }
}