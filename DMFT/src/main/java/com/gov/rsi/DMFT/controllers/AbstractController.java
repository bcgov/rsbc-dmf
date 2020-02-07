package com.gov.rsi.DMFT.controllers;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;

/**
 * Base class for all controller classes ("Controller.*"), providing common methods
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

//
//    /**
//     * Creates a 201 Created response with the key as body content and an Authorization header
//     * @param key the key
//     * @param token the token to be returned in the Authorization header
//     * @return the Response
//     */
//    protected Response responseCreated(Long id, String token){
//    	ResponseBuilder builder = Response.status(Response.Status.CREATED);
//    	builder.header("Content-Location", contentLocation);
//    	builder.header(HttpHeaders.AUTHORIZATION, "Bearer " + token);
//    	builder.entity(id);
//    	return builder.build();
//    }
//    
//    /**
//     * Creates a 201 Created response with the key as body content
//     * @param key the key
//     * @return the Response
//     */
//    protected Response responseCreated(Long id){
//    	ResponseBuilder builder = Response.status(Response.Status.CREATED);
//    	builder.header("Content-Location", contentLocation);
//    	builder.entity(id);
//    	return builder.build();
//    }
//    
//    /**
//     * Creates a 201 Created response with an array of keys as body content
//     * @param key the key
//     * @return the Response
//     */
//    protected Response responseCreated(List<Long> ids){
//    	ResponseBuilder builder = Response.status(Response.Status.CREATED);
//    	builder.header("Content-Location", contentLocation);
//    	builder.entity(ids);
//    	return builder.build();
//    }
//    
//    
//    /**
//     * Creates a 500 Internal Server Error response with no body 
//     * @return the Response
//     */
//    protected Response responseError(){
//    	ResponseBuilder builder = Response.status(Response.Status.INTERNAL_SERVER_ERROR);
//    	return builder.build();
//    }
//
//    /**
//     * Creates a 501 Not Implemented response with no body 
//     * @return the Response
//     */
//    protected Response responseNotImplemented(){
//    	ResponseBuilder builder = Response.status(Response.Status.NOT_IMPLEMENTED);
//    	return builder.build();
//    }
//
}