-> go_to_store

== go_to_store ==
Liora: Hi Mr. Michael!
Michael: Liora. Are you here to buy something?
Liora: Y-yeah. Are you alright? You seem upset about something.
Michael: It's nothing.
Liora: Alright.. I'll just be checking out the items over there.
Liora: I know that Mr. Michael has a daughter who lives in the capital. Maybe because the gate was closed, he's worried about her?
Liora: Huh, I see something shining over there... is that a gemstone?
Liora: Hey Mr. Michael! Where did you get that?
Michael: That's not for sale.
Liora: Why not?!
Michael: None of your business, kid.
    + [Steal it]
        -> steal
    + [Negotiate with a favor]
        -> favor

== steal ==
Liora: Mr. Michael! I saw a hooded person snag something and rush off over there!
Liora: He rushed off without a word... Okay, let's just throw this in my hat.
Michael: You touch anything?
Liora: N-no. I'll be going home now! Have a nice day.
Michael: Hm.
~ player_charisma -= 2
-> walk_away

== walk_away ==
Liora: That was close... I thought he was going to call me out there. 
Liora: Wait. It looks like Mr. Thomas is going up to the General Store? What's he doing here? He's usually holed up at the forge making armor. 
Thomas: I found what the material is. The fragment may look like a gemstone, but it's just a byproduct of magical energy. No worth whatsoever. I'm sorry.
Michael: *sigh* It's alright. Someone has already taken it anyways.
Thomas: What are you going to do now? Is Mary going to be okay?
Michael: I don't know. Her rent is getting more and more these days. I just have to keep selling.
Thomas: Well if you get any metal deposits I can pay extra.
Michael: ... Thank you, friend.
Liora: Mr. Michael looks super stressed and worried... 
    + [Return the gem]
        -> return_gem
    + {not return_gem} [Leave]
        -> leave
 
 == return_gem ==
 Liora: Mr. Michael! I'm sorry! I took the gem you had over there.
 Michael: It's fine. It's not worth anything. You can have it.
    + {steal} [Do him a favor]
        -> favor
    + {return_gem} [Leave]
        -> leave
 
 == leave ==
 ~ player_charisma -= 1
 { return_gem: 
    Liora: Okay, thank you Mr. Michael.
    Michael: Take care of yourself, kid.
    Liora: I hope everything goes okay with his daughter. Off to the gate I go!
    -> END
 }
 { not return_gem: 
    Liora: It's not my business and Mr. Michael said it wasn't worth anything anywas. Time to get this back to the gate!
    -> END
 }

== favor ==
{ steal:
    Liora: How about instead of this gem I find some herbs from the forest! I can totally find the rare ones that sell for a lot!
    Michael: Huh, if you can find some Dragon's Breath then I'd appreciate it kid.
    Liora: Of course, I can find it for sure!
    -> END
}
{ not steal:
    Liora: How about I get some herbs from the forest and you give me the gem? I bet I can find some rare ones that would be worth a lot!
    Michael: Hm. If you can manage to find some Dragon's Breath then maybe.
    Liora: You got it!
    -> END
}