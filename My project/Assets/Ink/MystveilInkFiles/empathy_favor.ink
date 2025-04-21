->  find_dragons_breath

== find_dragons_breath ==
~ player_charisma += 3
{ steal:
    Liora: I found some, Mr. Michael! Here you go. I hope your daughter is okay! I'm going to go now.
    Michael: Thank you. I'll let your mother know how much of a help you've been. Go along now. If you were that determined to make things right- I know you'll make a good Royal Guardsman.
    Liora: Thank you very much!
    Liora: Okay, time to go to the gate!
    -> END
}
{ not steal:
    Liora: I found some, Mr. Michael! 
    Michael: You really did it. Thank you, kid. I appreciate it. Here's the gem like you wanted.
    Liora: Yay! Thank you!
    Michael: ... She rushed off but, I'm sure she will be a good Royal Guardsman someday.
    -> END
}