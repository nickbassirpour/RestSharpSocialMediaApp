<H2>A way to post remotely to Reddit and Tumblr</H2>
<h3>This app will allow you to login to Reddit and Tumblr remotely to post content from one source.</h3>

<h4>Step 1: Register apps</h4>
<p>Register apps on both reddit and tumblr. </p>

<p>For reddit, <a href=https://github.com/reddit-archive/reddit/wiki/OAuth2>click here </a>for instructions on registering an app and how the api works. The redirect uri should be your localhost port address found in your launchSettings.json file with /reddit_get_token as the end route.</p>

<p>For tumblr, <a href=https://www.tumblr.com/oauth/apps>click here </a> to register an app:
The redirect uri should be the same local host port address with /tumblr_get_token as the end route.
For instructions on how the api works, <a href=https://www.tumblr.com/docs/en/api/v2>click here</a></p>


<h4>Step 2: Add env variables</h4>
<p>Navigate to the apps for both reddit and tumblr and copy/paste the keys to a .env file located in the root of your project.</p>
<p>Use these variables which are already being used in the app:</p>
tumblr_consumer_key,
tumblr_consumer_secret,
reddit_client_id,
reddit_client_secret

<h4>Step 3: Add redirect uri to .env file</h4>
<p>Add the reddit redirect uri that you previously used in registering the reddit app to your .env file.</p>
<p>Set the reddit redirect uri to this localhost address with this variable name: reddit_redirect_uri</p>
<p>(reddit requires it in their requests to verify it is the same as the app but tumblr does not. Feel free to add the same tumblr redirect here as well).</p>

<h4>Step 4: Use the app</h4>
<p>To use the app, turn it on, click the request permission for either api, agree to let them use permission, and then start posting.</p>
<p>You'll receive an access token that will be used to make requests and a refresh token that will be used automatically after 1 hour to get a new access token if you are still using the app.</p>
