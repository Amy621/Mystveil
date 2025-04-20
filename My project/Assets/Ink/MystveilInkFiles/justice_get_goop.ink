-> get_goop

== get_goop ==
Liora: Hey guys-
???: It's one of that Witch Gang's members!
Liora: Wait- I have this if you want it. Just stay and talk with me for a little bit.
???: Oooh, this is perfect for Operation: Slip n' Trip!
???: *sigh* Sorry about my twin. He's a little unhinged sometimes. What did you need, Liora?
Liora: You know my name? 
Nate: Of course. We're not "feral" like you guys think we are. We're normal kids just like you all. I'm Nate by the way. That's my twin Hilbert.
    + [Ask about the gemstone]
        -> ask_about_gemstone_nate
    + [Ask more about the misunderstanding]
        -> ask_about_the_misunderstanding
        
== ask_about_gemstone_nate ==
Liora: Have guys seen or taken a gemstone?
Nate: Gemstone? I think I saw Riley with one. Why?
Liora: You stole that from my friend!
Nate: Stole?
???: BROTHERS! Get away from that heinous monster!
Liora: What's happening?
Nate: *sigh*
Riley: WAHAHAH! Now that ME- Riley the Knight- is here, you will be slain!
Liora: ???
Nate: She's here about the gem.
Riley: Oh? What about it?
    + [Tell him that he stole it from my friend]
        -> interrogate_riley
    + [Ask more about Riley first]
        -> ask_about_riley


== ask_about_the_misunderstanding ==
~ player_charisma += 2
Liora: What do you mean by that?
Nate: By what?
Liora: That we all think you guys are "feral".
Nate: Isn't that the truth? You guys are all magic nerds and shun everyone who isn't like you.
Nate: Me and Hilbert don't want to be wizards or have anything to do with magic.
Nate: Our parents died from a magic battle between two high-level witches. The nightmares I get from the lights and shadows blasting in the distance still scare me.
Nate: A stray shadow caught my parents- and I never saw them again.
Liora: ... I'm sorry to hear that.
Nate: It was a while back. I'm happy here at the orphanage with the other kids now.
-> ask_about_gemstone_nate

== interrogate_riley ==
Liora: Have you taken a gemstone from someone around here?
Riley: Huh? Stole? I found this gemstone by the forest!
Liora: What?
Nate: Yeah, me and Hilbert were there as well.
Liora: What does this mean... Rose... lied?
Myrtle: Liora?
Liora: Myrtle? What are you doing here?
Myrtle: Everyone is inside Biz's right now. They will be coming out in a second.
Myrtle: I wouldn't be seen associating with them Liora...
Nate: ...
    + [Defend the orphans]
        -> defend_orphans
    + [Go to your friends' side]
        -> go_to_friends_side

== ask_about_riley ==
Liora: You're Riley, right? Are you also an orphan.
Riley: Yep! BUT not just that, I am the HOLY KNIGHT of Legend!
Liora: Holy Knight?
Riley: You haven't heard the myths? The Knight will come and save everyone with the Legendary Sword! I will become that Knight and save everyone!
Liora: ???
Nate: *sigh* Just ignore everything this guy says. He is a younger kid in the orphanage and is obessed with Knights.
Liora: Oh, I see. I know that there have been knights in the distant past who used to travel with party members of different races and abilities.
Nate: Yeah, it's from those stories that he became obsessed.
Liora: But nowadays, knights are extinct- or at least extremely rare. 
Nate: You can probably ask him about the gem now, even if he is enthusiastic he listens to people.
-> interrogate_riley

== defend_orphans ==
~ player_charisma += 8
Liora: These orphans are not "feral"! They're normal kids just like us.
Rose: ... Is that how it is?
Liora: Rose...
Rose: You're going to betray us, after everything we did and went through?
Liora: Rose. You lied to me. You lied and just want the gem for yourself.
Rose: ...
Nate: You're really defending us?
Liora: I can't have them hating on you for no reason.
Rose: You idiot. Just wait until I get back from the Magic Academia.
Myrtle: *bows before running away*
Natalie: ... I hope you know what you're doing girl.
Henry: *sigh* I trust your decision but I have to go now. Good luck Liora.
Liora: ... They're gone.
Nate: Thank you Liora. Really.
Liora: Of course! 
Hilbert: We still can't trust her- she's a witch!
Riley: I say she embarks on a JOURNEY to prove her INNOCENCE!
Nate: ... What he said. He's the one who found the gem anyways.
Liora: Okay, that's fine by me!"
Riley: If you can defeat the Lunar Fenrir and collect its items then you have proven your loyalty to the Werewolf Gang!
Liora: Be right back!
-> END

== go_to_friends_side ==
~ player_charisma -= 5
Liora: ...
Rose: Oh, Liora! Fancy seeing you here. What are you doing?
Liora: I was trying to get the gem back from these three.
Rose: I didn't see the trash standing there- let's just grab it and go. It doesn't belong to dirty and feral monsters.
Nate: ... Hah, I expected this to happen. Just take it and leave us. 
Hilbert: Nate...
Riley: ....
Henry: Please leave the area. This is by my parents restaurant.
Nate: We're going.
Natalie: They're finally gone.
Rose: Ugh, it has their dirt on it! Let's clean it off and then it's all yours Liora!
Myrtle: You did the right thing Liora.
Liora: ...Thank you.
-> END