@echo off

echo Updating client

rem removed --add-credentials for testing

autorest --verbose --input-file=icbc-adapter.swagger --output-folder=.  --csharp --use-datetimeoffset --sync-methods=all --generate-empty-classes --override-client-name=IcbcClient --legacy --namespace=Rsbc.Dmf.Interfaces.IcbcAdapter --debug
