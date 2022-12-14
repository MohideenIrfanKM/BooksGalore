# BooksGalore
BooksGalore is a E-Commerce Web Application created by using DotNet Framework(.net 6)

About:
This Web Application is created with MVC(Models-Views-Controllers) architecture and on the top of that, this projects also implements REPOSITORY PATTERN with the help of 
DEPENDENCY INJECTION container which seperates all the data accessible codes with other codes and reduce the code redundancy which also makes this project clean. The user
data and all book details are stored in MSSql database.I have followed Code First approach to create ORM first and Create relevant tables in the database by performing 
Migrations.Email features are added with SMTP protocol.

Welcome to BooksGalore, a web application for online book ordering and processing.

Features:

Email verification and notification upon login or order placement
Payment processing through Stripe checkout
Social media login and registration options
Four types of user accounts: Admin, Employee, Individual, and Company
Easy-to-use interface for adding and maintaining book details
Easy-to-use interface for Managing different orders



Requirements:

.NET 6 framework
MSSQL database

Important Nuget Packages:

Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.Extension.Identity.Core
Stripe.net
MimeKit and MailKit

Installation:

Clone the repository to your local machine
Install the necessary dependencies
Update the database connection string in the appsettings.json file
Build and run the project

Usage:

Register for an account as an Individual, Company, Admin, or Employee
Log in to the application
If you are an Admin or Employee, you can add and maintain book details
If you are an Individual or Company, you can browse and order books
Upon successful payment, your order will be processed and shipped
We hope you enjoy using BooksGalore and find it a convenient way to order books online. Thank you for choosing my application.
