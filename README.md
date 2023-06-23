
# Twitch Drops Farmer

A scuffed program that will farm twitch drops for you.

***How it works?***

It checks every x minutes (default 1 minute) if any of the target streamers are live if they are then a browser window will be opened with their stream running until either the drop is obtained or the streamer goes offline. If the streamer goes offline then the stream will continue after they are back online.


## Usage

1) Download this program.
2) Go to https://dev.twitch.tv/console and login with your twitch account.
3) Over on twitch developer portal click (Register your Application) unless you already have one.
4) Fill the details:
- Name: (Whatever you want)
- OAuth Redirect URLs: http://localhost
- Category: (Can leave empty or just pick something)
5) Click "Create"
6) Back over at the console click "Manage" behind your application
7) Start this program and enter password (If its your first time using this then put a password you will use to encrypt the details.)
8) Now go to "Change Settings" and choose option "Client ID" and from twitch developer console copy your Client ID.
9) Do the same for "Client Secret". Press "New Secret" to get it.
10) Now in the application choose "Encrypt Config Values/Save Config"
11) Now choose "Generate Access Token (OAuth)" and wait for it to be done.
12) Now again choose "Encrypt Config Values/Save Config".
13) Add streamers to list, change the settings as you prefer and enjoy the bot.
14[optional]) Use a script that auto-claims twitch drops for you. Otherwise there could be some problems with fe. generic drops in rust

#
Expect there to be bug and possibly some crashes. Please let me know of any problems you encounter.

**AS OF RIGHT NOW IT OPENS THE STREAM IN YOUR DEFAULT BROWSER**
