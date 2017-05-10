## Conversation with the original developer

**Justin Ng**
> So, I've decided to give up on my little chart analyzer for the time being. Mostly because I'm swarmed with a lot of other pressing matters (like work, government paperwork and a university application).
The other huge reason is that I feel like I've hit the boundary with what knowledge I have where graph search and pruning algs are concerned. Also, I haven't been able to put together the heuristics right for the costing.
The code I have currently is pretty darn ugly and doesn't work perfectly. If anyone's interested in the code, here it is (I'm pretty sure no one gives a damn): https://www.dropbox.com/s/rluuaei3gc2pr8u/SSC-AI.zip?dl=0
Forgive the poor naming conventions, poorly chosen names and poor coding standards and poor whatever-else.
If there's at least one other dude who cares, you can create your own solvers by implementing ISolver, ICostFactory and ICost. Then, if there's anything else you want to tweak and whatnot, you'll have to go down to the lower levels of the code.
I've been trying my hand at implementing a chart analyzer for a long while now, on and off. This is attempt no. god-knows-how-many.

>Maybe I'll eventually finish it. But if someone else can do it before I can, I'll be happier.

>The last thing I was working on, before giving up, was getting the analyzer to "play" the ending brackets of Hypnosis (Synthwulf Mix) D20 correctly.

>Before that, I got gallops to work after some costing tweaks. The analyzer, so far, (generally) chooses the correct alternating feet style of play and avoids facing backwards and double steps.'

**Jhonatan A.**

>so, officially open sourced?

**Justin Ng**

>Eh, do whatever you want with it, it's all undocumented and it might cause your computer to explode.
Open source, free source, weak sauce, BBQ sauce

**Jhonatan A.**

>then: 
[Do What The F*ck You Want To Public License v2 (WTFPL-2.0)](https://tldrlegal.com/license/do-wtf-you-want-to-public-license-v2-(wtfpl-2.0)#fulltext)

**Justin Ng** 
>:thumbsup:

>I wouldn't try the dfs solver as is, though. I noticed a bunch of bugs with it.

>The BFS with heuristics isn't much better either, actually. I'd recommend DFS with a good pruning alg at this point but I got too burned out to implement and test.
