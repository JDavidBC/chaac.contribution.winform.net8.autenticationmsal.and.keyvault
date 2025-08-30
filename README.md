# chaac.contribution.winform.net8.autenticationmsal.and.keyvault
This project is the initial configuration for a winformproject with Azure EntraId User Authentication and using keyvault to read connectionstring for the database.

# Azure EntraId Contiguration

You need to register a new Application.

<img width="461" height="93" alt="image" src="https://github.com/user-attachments/assets/e91421f4-0214-4aed-b3da-c4cddee7cf6c" />

You need to define: Redirect URI (yeah, even if we are creating a winform application)

<img width="992" height="140" alt="image" src="https://github.com/user-attachments/assets/9be90aee-b6cf-4b3b-a1b3-98b43811e8d2" />

<img width="528" height="74" alt="image" src="https://github.com/user-attachments/assets/61353fea-fb20-4d79-b24a-9fa960de49dc" />

yes, this URL must exist in your redirections.

For my applications I always select 

<img width="289" height="107" alt="image" src="https://github.com/user-attachments/assets/7f99ada8-d3b7-490c-87c3-8c9d4d71e752" />


<img width="667" height="121" alt="image" src="https://github.com/user-attachments/assets/53a2556a-b201-4d0a-8b9f-e2d51d34a567" />


# Application Configuration to access Azure Key Vault.

Remember, your application need permissions to connect to Azure KeyVault

<img width="315" height="225" alt="image" src="https://github.com/user-attachments/assets/efba677c-df53-4a1d-8c1d-f8ab9dd2b2ea" />

Add this configuration

# Configuration of Azure Key Vault

You must have created one keyvalut with the right secrets there. "ConnectionString" for example.

Go to Access policy

<img width="240" height="209" alt="image" src="https://github.com/user-attachments/assets/1f2ea268-e2e3-4c13-8c96-d84d9cdcf1a8" />

Create the right permissions for the users/or group of users.

<img width="817" height="417" alt="image" src="https://github.com/user-attachments/assets/c8647586-ffed-458f-b677-73f8716847a7" />


# Configure the right values in the code.

In applicationsettings.json you will see all the values you need to add there. Add it.

# Run de application

You should ve able to see Form1 as initial form, the log in the right folder with the name of the user logged in.

# DI

in this application you have DI created to avoid to use Ninject, Autofact, etc. with the configuration for IDbConnection (needed for Dapper), and Serilog sink to file, every day.





