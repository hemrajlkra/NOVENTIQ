# NOVENTIQ



**Register new User:** Anonymous user can register with default role EMPLOYEE
POST
API : https://localhost:44346/api/Auth/register
Body Json: 
{
  "name": "string",
  "email": "string",
  "phoneNumber": "string",
  "password": "string",
  "role": "string"
}

**Login with email password:** Token will be created for each user once login successfull which will have claims and token expiry.
POST
API : https://localhost:44346/api/Auth/login
Body Json:
{
  "username": "string",
  "password": "string"
}

**Assign Role to existing User:**
POST
API : https://localhost:44346/api/Auth/assignrole
JsonBody: 
{
  "email": "string",
  "password": "string",
  "role": "string"
}

**Refresh token for User:** Once a token is refreshed it can't be refreshed again.
POST
API: https://localhost:44346/api/Auth/refreshtoken
JsonBody:
{
  "refreshToken": "string"
}

**Get All User**: This is accessed by authorized user with admin role only with the token passed in header.
GET
Api: https://localhost:44346/api/User/getall

**Create User:** Only User with admin role can create a user
POST
https://localhost:44346/api/User/create
{
  "name": "string",
  "userName": "string",
  "email": "string",
  "phoneNumber": "string",
  "password": "string"
}

**Get User based on Email** : This can be accessed by user with role EMPLOYEE or ADMIN
GET
https://localhost:44346/api/User/getuser?email=<emailid>

**Update User**: Only user with admin role can update the user.
PUT
https://localhost:44346/api/User/update?id=""
{
  "name": "string",
  "email": "string",
  "userName": "string",
  "phoneNumber": "string"
}

**Delete User**: User with Admin role can delete user.
DELETE
https://localhost:44346/api/User/delete/{id}
