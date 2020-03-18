import React, { Component } from 'react';
import axios from 'axios';

class App extends Component {

    state = {}
    
    constructor(props){
        super(props);
        this.fetchData = this.fetchData.bind(this);
    }

	componentDidMount() {
        let baseURI = document.baseURI;
        if (!baseURI) {
            // For IE
            baseURI = window.location.href;
        }
    
        console.log(baseURI);
        if (baseURI === "http://localhost:3000/"){
            // Client loaded from VSCode local server 
            axios.defaults.baseURL ="http://localhost:8080/"; 
        }
        else {
            // Client loaded from local or remote Spring Boot application
            axios.defaults.baseURL =  baseURI;
        }
        this.fetchData();
    }

    fetchData() {
        axios.get('/api/queue/dmer/status')
		.then( response => {
            this.setState({ "dmers": response.data })
        })        
    }

	render() {

        const header = {
            color: "white",
            backgroundColor: "black",
            padding: "10px",
            fontFamily: "Arial",
            width: "200px"
        };
        const cell = {
            color: "white",
            backgroundColor: "DodgerBlue",
            padding: "10px",
            fontFamily: "Arial",
            width: "200px"
        };
        const cellClean = {
            color: "white",
            backgroundColor: "green",
            padding: "10px",
            fontFamily: "Arial",
            width: "200px"
        };
        const cellInProcess = {
            color: "BurleyWood",
            backgroundColor: "green",
            padding: "10px",
            fontFamily: "Arial",
            width: "200px"
        };
        const cellOther = {
            color: "BurleyWood",
            backgroundColor: "red",
            padding: "10px",
            fontFamily: "Arial",
            width: "200px"
        };


        const formatter = new Intl.DateTimeFormat('default', {
            hour: 'numeric',
            minute: 'numeric',
            second: 'numeric'
        });
      
		return  (
      <>
        <h2>Driver Medical Examination Report (DMER)</h2>
        <div>
            <input type='button' value='Refresh' onClick={this.fetchData}></input> 
        </div>
        <table style={{padding: "10px"}}>
            <thead>
                <tr>
                    <th style={header}>DLN</th>
                    <th style={header}>Status</th>
                    <th style={header}>Name</th>
                    <th style={header}>Time</th>
                </tr>
            </thead>
            <tbody>
            {
            this.state.dmers ?
            this.state.dmers.map((dmer) => {
                    return (
                        <tr key={dmer.id}>
                            <td style={cell}>{dmer.licenseNumber}</td>
                            <td style={
                                dmer.status === 'NEW' ? cell :
                                dmer.status === 'CLEAN_PASS' ? cellClean : 
                                dmer.status === 'IN_PROCESS' ? cellInProcess : cellOther
                                }>{dmer.status}</td>
                            <td style={cell}>{dmer.displayName}</td>
                            <td style={cell}>{
                                dmer.status === 'NEW' ? dmer.timeNew : 
                                    dmer.status === 'IN_PROCESS' ? dmer.timeInProcess : dmer.timeFinished
                                }
                            </td>
                        </tr>
                    )
                })
            :
            ''
            }
            </tbody>
        </table>
			</>
    )
    }
}

export default App;
