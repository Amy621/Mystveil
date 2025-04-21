// Example Ink script for quest dialogues
// This demonstrates how to use external quest functions from Ink

// External functions we can call
// EXTERNAL startQuest(questName)
// EXTERNAL completeObjective(questName, objectiveDescription)
// EXTERNAL isQuestActive(questName)
// EXTERNAL isQuestComplete(questName)
// EXTERNAL getObjectiveProgress(questName, objectiveDescription)

INCLUDE globals.ink

VAR playerName = "Adventurer"

-> main

=== main ===
// Check quest status and show appropriate dialogue
{ isQuestComplete("Gather Herbs"): 
    -> quest_complete
- isQuestActive("Gather Herbs"):
    -> quest_active
- else:
    -> quest_not_started
}

=== quest_not_started ===
Guard: Greetings, {playerName}! The town's alchemist needs herbs for potions.

* [I can help]
    Guard: Excellent! The alchemist needs 5 different herbs.
    ~ startQuest("Gather Herbs")
    Guard: You can find these herbs in the forest to the east. Return to me when you've collected them all.
    -> END
    
* [Not interested]
    Guard: Very well. Come back if you change your mind.
    -> END

=== quest_active ===
// Check objective progress
{ getObjectiveProgress("Gather Herbs", "Collect herbs") < 5:
    Guard: How's the herb collection going? The alchemist still needs more herbs.
    Guard: You've collected {getObjectiveProgress("Gather Herbs", "Collect herbs")} out of 5 herbs so far.
    
    * [I found another herb (simulate finding one)]
        ~ completeObjective("Gather Herbs", "Collect herbs")
        Guard: Great! That's {getObjectiveProgress("Gather Herbs", "Collect herbs")} herbs now.
        -> END
        
    * [I'll keep looking]
        Guard: Keep searching in the forest to the east.
        -> END
- else:
    Guard: Excellent! You've found all the herbs the alchemist needs.
    ~ completeObjective("Gather Herbs", "Return to the alchemist")
    Guard: The alchemist will be pleased. Head to the alchemy shop to deliver the herbs.
    -> END
}

=== quest_complete ===
Guard: Thanks again for helping with those herbs. The alchemist has been making excellent potions thanks to your efforts.

* [Happy to help]
    Guard: The town values adventurers like you. Stop by later, I might have more work for you.
    -> END
    
* [Do you have any other quests?]
    Guard: Not at the moment, but check back later.
    -> END 