Use common.proto for common messages and enums

To add a new proto file:
1. Add a .proto file in the 'Rsbc.Dmf.CaseManagement.Service.Protos' directory								
2. Right-click the file and select 'Properties', then set the 'Build Action' to 'Protobuf Compiler'
3. In 'Properties' set 'gRPC Stub Class' to 'Server'
4. Edit 'Rsbc.Dmf.CaseManagement.Client' project file and add the file to the item group, use cmsAdapter.proto as a template
