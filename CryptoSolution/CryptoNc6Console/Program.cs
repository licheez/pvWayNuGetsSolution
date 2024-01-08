using CryptoNc6Console;
using pvWay.Crypto.nc6;

Console.WriteLine("Hello, pvWay Crypto nc6");
Console.WriteLine();

// the length of the key string needs to be exactly 32
const string keyString = "123456789 123456789 123456789 12";

// the length of the init vector string needs to be exactly 16
const string initializationVectorString = "0123456789ABCDEF";

// Factory the crypto object
// note that we mention 10 minutes as the default validity
// time for ephemeral objects or strings
var crypto = PvWayCryptoDi.Create(
    keyString,
    initializationVectorString,
    TimeSpan.FromMinutes(10));

Console.WriteLine("Encrypting the word 'test'");
var b64 = await crypto.EncryptStringAsync("test");
Console.WriteLine($"The word 'test' was encrypted as '{b64}'");
Console.WriteLine("Retrieving the 'test' from the encrypted payload");
var text = await crypto.DecryptStringAsync(b64);
Console.WriteLine($"Got '{text}' back");
Console.WriteLine();

var mc = new MyClass
{
    TheHeader = "Header",
    TheBody = "Body",
    TheFooter = "Footer"
};
Console.WriteLine("Encrypting the object MyClass(Header, Body, Footer)");
b64 = await crypto.EncryptObjectAsync(mc);
Console.WriteLine($"The MyClass object was encrypted as {b64}");
Console.WriteLine("Retrieving the MyClass object from the payload");
var mcBack = await crypto.DecryptObjectAsync<MyClass>(b64);
Console.WriteLine($"Got MyClass back '{mcBack.TheHeader} - {mcBack.TheBody} - {mcBack.TheFooter}'");
Console.WriteLine();

Console.WriteLine("Encrypting the object MyClass(Header, Body, Footer)");
Console.WriteLine("Payload will only stay valid for 1 second");
b64 = await crypto.EncryptEphemeralObjectAsync(
     mc, TimeSpan.FromSeconds(1));
mcBack = await crypto.DecryptEphemeralObjectAsync<MyClass>(b64);
Console.WriteLine("Retrieving the MyClass object from the payload");
Console.WriteLine($"Got MyClass back '{mcBack!.TheHeader} - {mcBack.TheBody} - {mcBack.TheFooter}'");
Console.WriteLine("Ephemeral payload is still valid");
Console.WriteLine("Sleeping a while");
Thread.Sleep(1000);
Console.WriteLine("Retrieving the MyClass object from the payload");
mcBack = await crypto.DecryptEphemeralObjectAsync<MyClass>(b64);
if (mcBack != null)
    throw new PvWayCryptoException("Payload should have expired");
Console.WriteLine("The object payload has expired as expected");
Console.WriteLine();

Console.WriteLine("Encrypting the ephemeral text 'Hello butterfly' " +
                  "that will only stay alive for 1 second");
b64 = await crypto.EncryptEphemeralStringAsync("Hello butterfly", TimeSpan.FromSeconds(1));
Console.WriteLine("Retrieving the ephemeral text");
text = await crypto.DecryptEphemeralStringAsync(b64);
Console.WriteLine($"The text '{text}' is still valid");
Console.WriteLine("Sleeping a while");
Thread.Sleep(1000);
text = await crypto.DecryptEphemeralStringAsync(b64);
if (text != null)
    throw new PvWayCryptoException("Payload should have expired");
Console.WriteLine("The text payload has expired as expected");
