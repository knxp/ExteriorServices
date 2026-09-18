Why I'm building this app:
I'm starting a company where we will be doing exterior services. Mainly Pressure washing (cleaning houses, cars, boats, ect.) and Christmas Light decoration (buying lights, decorating someones house, taking them down later).

My priorities have switched recently. We plan to start knocking doors within the next week. I have to have a majority of this application setup and live.

Look at the entire solution I have setup now. I have an API, Application, Domain, Infrastrucutre, and Web setup so far. Our API and DB can handle Customers, Properties, and Customer Properties. I'm not interested in a client facing website for right now. I'm focused on our portion of the software that will help us get clients and hold the data.

Here is what I need built within the next week.

I need an Admin Dashboard that will be accessible from a Phone or IPad. This Dashboard needs a clean and clear UI. The Dashboard will be used primarily for 3 things.

1. User input for customer information logging. I will be using this screen to input information realtime whenever a customer agrees to the service. I will a screen or tab where i can input a new customer and one where I can update an existing customer. I want to be able to input all of the fields that customer and property already have, and I also want to be able to attach images of the property to the client.
2. Image generation. I must have a tab where I can take a photo of the clients house and upload it to the page. From here, I want to generate an image using openAI chatgpt image generation API to modify the photo of the clients house, to show how it would look at night with the lights and decor that we specify. Ideally there will be check boxes or drop down menus to select from. I also want to be able to place a reference item (likely a spray painted yard stick) in one of the photos, and have the AI give an estimated measurement of how many feet of lights we will need. This will come after the night time image.
3. View analytics and client information easily. Searchable and readable and filterable. I want this to mimic how most professional CRMs work. It doesn't need to infer any information, just give us detailed info whenever we need it. Quick runtime, efficient, and solid.

I dont have a domain quite yet, i will buy after local is good. I will be using azure to host all of this. I will be using openai api for image generation.
