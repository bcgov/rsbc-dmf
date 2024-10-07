NOTE!!! Try using aspnet dev certificate instead and delete this ssl folder. The apsnet dev certificate will already be signed and is a better candidate since I'm not able to sign my own certificate on Windows 11

# Generate SSL Certificate

Run the following command in the ssl folder:
`openssl req -new -x509 -newkey rsa:2048 -sha256 -nodes -keyout localhost.key -days 3560 -out localhost.crt -config cert.cnf`
(NOTE confirmed working for Git Bash for Windows but should work in any terminal if openssl is installed)

# Install Certificate

## Windows 10/11

- Double-click on the certificate (server.crt)
- Click on the button “Install Certificate …”
- Select whether you want to store it on user level or on machine level
- Click “Next”
- Select “Place all certificates in the following store”
- Click “Browse”
- Select “Trusted Root Certification Authorities”
- Click “Ok”
- Click “Next”
- Click “Finish”
- If you get a prompt, click “Yes”
- Restart browser

## For OS X

- Double-click on the certificate (server.crt)
- Select your desired keychain (login should suffice)
- Add the certificate
- Open Keychain Access if it isn’t already open
- Select the keychain you chose earlier
- You should see the certificate localhost
- Double click-on the certificate
- Expand Trust
- Select the option Always Trust in When using this certificate
- Close the certificate window
