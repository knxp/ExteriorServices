Now that the site is deployed and working there are a few changes that I want to make.

Whenever an image is generated for a house, sometimes it comes back and its missing a roof ridge, or it added extra bush or the lights are wrong ect. After an image is generated and shown in the preview, I want to be able to make 1 revision to that image. After we get the preview, there should be 2 buttons, [MAKE REVISION] and [LOOKS GOOD]. If we click make revivsion, then we should have a notes popup box, where we will manually type in the revisions that we want. we should have a short default string that we feed, something like "take this edited image and make the following minor adjustments" and then in the notes I will manually type per each house generated a custom "take the lights off of the side of the house, and make sure to put lights on the top roof line."

We should have an "Are you sure" button after you press submit on make revision and also looks good button. if a revision is made, we should also hold on to that image and have it linked to the property/customer.

I also want to try this. I want to see if openai can estimate the total footage of lights that it places on the house. I dont know the best most efficient route for this. I want to choose the route that would use the least amount of credits for the api. I dont know if the image gen ai can send back text or nuumbers. But i want a way to estimate the total footage of lights for c9s only. based on the normal size of doors, garage doors, camera angles, ect so that we can have a rough estimate of the light footage so we can give an accurate quote. I dont know if this is possible without some sort of reference object in the photo, but i think it could be done without it. lets think about this one before we code it. we would need to add a column to the property table called "LightFootage"

another tweak that is needed is this: on the images page, each image has the date and size, which is good. but the name is a random string like 9250ba3a. we should make the image library card body the name or the address and make the card itself clickable, with all the information that we have for that house, including property, customer, job. it doesnt have to be big and bulky, just listed out, similar to the calendar when a job is clicked.

answers and thoughts:

yes we should give it the already edited one, since most times its 90% good, it just needs a minor tweak that was missed or halucinated.

Youre right, LightFootageEstimate should be the name and it should be with visualization and then added to the property when approved.

i like the trigger point after looks good.

llightfootage should only run after looks good. we should even have a "estimate footage" button after the looks good and maybe even on the imagges tab for later. for example if we just wanted to show how it looks to the customer at first, they say maybe, then we want to give an estimate a few hours later when they say yes, we then go in and do the estimate later on. i would just like the option to yes/no estimate instantly after the final looks good, but also later on after we've moved to a different house and go back to it.
