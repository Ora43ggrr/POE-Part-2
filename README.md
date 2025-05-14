Project details
Project name:ST10440381 Part 1
Netframe4.7.2
Template :Console App c# netframework

1 Keyword recognition
-User input is checked against predefined patterns using lamda expressions
-When the user eneters a question a keyword is detected o rather recognised (Privacy,password and phishing), the corresponding handler delegate is invoked
-Each handler provides topic specific response with variations
-The user will give responses based on the keyword of the related topics

Example
User(You):"Tell me about password safety"
Chatbot recognizes "password" and invokes passwordHandler
Response(Chatbot):"make sure to use strong , unique password sfor ecah account
 
2 Random Responses 
-Responses are organized in nested lists by topic and response set
-a counter trcaks how many times ecah topic has been discussed
-the counter then determines which response set to use
-then a random response is selected from the approriate set
-Even when the user asks the same question the chatbot is able to give different responses 

3Conversation Flow
-The chatbot is able to track conversation history and when the user wants the converation at a later stage its going to display what the user and chatbot have been interacting uses:private List<string>conversationHistory = new List<string>();
-Tracks also user interests
Follow up handling
-The chatbot is able to remember previous topics and can reference them
Example: "Earlier we discussed passwords. Would you like more tips on that?"

4 Memory and Recall
- User details are stored in a text file(ChatbotMemory.txt)
-The information is then categorized by type(name, interest)
-The chatbot is also able to recall and reference stored information later
-Example:" I remember you interested in privacy.Here's a new tip..."

5 Sentiment Detection
-The chatbot detects user sentiment using keyword matching
-So according to the users tone or "emotions" the Ai will adjust the response and like give you reponse based on the sentiment by the user
Example
User(You): "Im angry about phishing "
Chatbot: " I understand your frustration", then proceeds o give response

6 Error Handling and edge Cases
-The chatbot will handle unexpected input gracefully
-if it does not understand what the user is saying the  it will display a message saying:"I'm sorry, I dont understand that question.Please ask about cybersecurity"
-Default response for unrecognized input
-Input validation for example name validation
-File operation handling
-Empty input handling

7 Code Optimization
This implementation uses effiecient data structures
1 Dictionaries for quick keywords lookup
2 List for managing responses variations
3Delegates for flexible response handling


