# CS441_Vac_Sim

The main purpose of this exercise is to recognize how inefficient relex agents are. It is very difficult to code every single situation or scenario that the agent is able to come into contact with (which would be called a Rule Table). The primary shortcoming of a reflex agent is its inability to store memory, so it has no idea where it's been, yet is just able to take a "snapshot" of the world in it's current state, for each state that the agent is in.

I really enjoyed this assignment because it showed me how difficult it is to design a solid algorithm for a reflex agent, or more specifically, designing the options in the rule table. That was the most interesting thing to me and having spent the time in building a reflex agent, I see first hand how impractical it is to build. The one thing I was having a hard time with was making the agent read the entire work around them. Since a reflex agent cannot store information, it has to respond to a given “snapshot” of the surrounding environment for each stage it’s in, it makes it very inefficient. Let’s take a look at this scenario where the agent is trapped in between two piles of dirt. I have four priority lists for the agent.

* The first priority list is:
    * Look at all squares to your right, is there a dirt pile? Go right one.
    * Look at all squares below you, is there a dirt pile? Go down one.
    * Look at all squares to your left, is there a dirt pile? Go left one.
    * Look at all squares above you, is there a dirt pile? Go up one.

Then there are three other priority lists as well and the agent cycles through them.
* These aren’t the same as the rule table, since the rule table only has six options:
    * Suck up dirt
    * Move right
    * Move down
    * Move left
    * Move up
    * Declare “All Clean!” (No Op)

My focus wasn’t in the rule table, yet the priority of numbers 2-5. There were many scenarios where the agent would get stuck, going back and forth between two piles of dirt like in the image below. This turned out to make a lot of repeat motions, turning out to be very inefficient. To fix this, I altered the list 2-5 depending on the count of moves in the current simulation. It’ll cycle through four different priority lists. The major hurdle was the fact that a truly reflexive agent can only make a “snapshot” of the current world environment for each state that it’s in and it cannot have memory. The snapshot as described in Figure 2.11 of the book is “What the world is like now” which is how the agent is able to examine its surroundings.

The chart below shows that the simple prioritization caused the agent to outperform the random agent drastically.

![resulting data](https://github.com/williammcintosh/CS441_Vac_Sim/blob/main/images/Screen%20Shot%202021-06-26%20at%2011.06.04%20PM.png)
