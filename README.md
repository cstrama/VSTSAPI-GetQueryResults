# VSTSAPI-GetQueryResults
This is a C# console application that gets a query saved in VSTS (typically under Shared Queries) and returns the Work Items and details of the work item.

Prerequisites:
1. Shared Query saved in VSTS (https://www.visualstudio.com/en-us/docs/work/track/using-queries).
2. VSTS Personal Access Token (PAT) (https://www.visualstudio.com/en-us/docs/setup-admin/team-services/use-personal-access-tokens-to-authenticate).
3. The ID of the Shared Query (https://www.visualstudio.com/en-us/integrate/api/wit/queries#GetaqueryorfolderQuerybyID).

Note: Potentially, you can also write the WIQL and encode it using a third-party tool, such as Fiddler.

To use:

1. Replace Authorization line with username:PAT encoding from Fiddler (To Base64) (2 places).
2. Update query strings with your account (i.e., {youraccount}.visualstudio.com).

test update
