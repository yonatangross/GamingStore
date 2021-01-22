## GamingStore
The Gaming Store project is a website for a gaming store with an online and pyshical shops.
It has an administration control panel for chain store manager & store managers, and a client side for clients to buy in.

The website was built with ASP.NET Core MVC Web Application.

## Motivation
The application motivation is to create an web application that serves both the customers and store managers.

## Tech/framework used
<b>Built with</b>
- [ASP.NET Core 3.1](https://docs.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-3.1)

## Features
1. Items recommender with [ML.NET](https://dotnet.microsoft.com/apps/machinelearning-ai/ml-dotnet) based on previous orders and users choices.
2. UI & UX design for the control panel, user profile and the website using Bootstrap, CSS & SCSS, ,JS, jQuery & D3.js.
3. Mock quality data for the store, customer and item models using [Mockaro](https://www.mockaroo.com/).
4. Maps using Google Maps API's(Maps JavaScript API & Geocoding API) for pinpointing stores addresses on a map for customers comfort.
5. Graphs for store revenues & items statistics using [d3js](https://d3js.org).
6. Email Service using [MailKit](https://github.com/jstedfast/MailKit) in order to handle user authentication, forget password, account confirmation, contact us and other profile related actions. 
7. AJAX to asynchronously search and edit users in the control panel.
8. Publication of new items in Twitter using Twitter API.
9. Authentication of users with Google & Facebook for easy registration and login.
10. Supplying 2FA(2 factor authentication) with an authentication for further securing the users account.

## Screenshots
| | |
|:-------------------------:|:-------------------------:|
|<a href="https://ibb.co/4VXw9yY"><img style="max-width:200px; width:100%"  src="https://i.ibb.co/RDsGZnB/Gaming-Store-Control-Panel1.png" alt="Gaming-Store-Control-Panel1" ></a>|<a href="https://ibb.co/pv1sVb4"> <img style="max-width:200px; width:100%"  src="https://i.ibb.co/JRcb6mr/Gaming-Store-Control-Panel2.png" alt="Gaming-Store-Control-Panel2" ></a>|
|<a href="https://ibb.co/QCwkQfy"><img style="max-width:200px; width:100%"  src="https://i.ibb.co/yYjpPXH/Gaming-Store-Stores-List.png" alt="Gaming-Store-Stores-List" ></a>|<a href="https://ibb.co/Ybn8Zvm"> <img style="max-width:200px; width:100%"  src="https://i.ibb.co/Ws8FtSZ/Gaming-Store-Contact-Us.png" alt="Gaming-Store-Contact-Us" ></a>|
|<a href="https://ibb.co/FJ3fDd9"><img style="max-width:200px; width:100%"  src="https://i.ibb.co/6bwpWq2/Gaming-Store-Control-Panel3.png" alt="Gaming-Store-Control-Panel3" ></a>|<a href="https://ibb.co/WWN2kVY"> <img style="max-width:200px; width:100%"  src="https://i.ibb.co/TBDg4WF/Gaming-Store-Control-Panel4.png" alt="Gaming-Store-Control-Panel4" ></a>|||
|<a href="https://ibb.co/fpxv6V0"><img style="max-width:200px; width:100%"  src="https://i.ibb.co/tsJXjwC/Gaming-Store-Products.png" alt="Gaming-Store-Products" ></a>|<a href="https://ibb.co/Db86x4L"> <img style="max-width:200px; width:100%"  src="https://i.ibb.co/ky9rRKM/Gaming-Store-Shopping-Cart.png" alt="Gaming-Store-Shopping-Cart" ></a>|
|<a href="https://ibb.co/chkHmgD"><img style="max-width:200px; width:100%"  src="https://i.ibb.co/3m4qJrB/Gaming-Store-Control-Panel5.png" alt="Gaming-Store-Control-Panel5" ></a>|<a href="https://ibb.co/1JLk1B4"> <img style="max-width:200px; width:100%"  src="https://i.ibb.co/VptsRbk/Gaming-Store-Cart.png" alt="Gaming-Store-Cart" ></a>|

## Installation
0. Make sure you have secrets.json on your computer in %APPDATA%
1. Remove migrations folder
2. Drop-Database
3. write following commands on PMC:
3.1 Add-Migration InitialCreate
3.2 Update-Database
4. run program.

## API Reference
- [Google Maps API](https://developers.google.com/maps/documentation)
- [Facebook API](https://developers.facebook.com/)

## Credits
 Course Instructor [Shay Horovitz](https://www.linkedin.com/in/shay-horovitz-25bb31/)
## License
MIT © [Yonatan Gross](https://github.com/yonatangross), [Ohad Cohen](https://github.com/OhadCohen97), [Aviv Miranda](https://github.com/Aviv943), [Matan Hassin](https://github.com/AnubisMatan)

