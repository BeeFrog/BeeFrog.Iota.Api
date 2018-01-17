
# Attention

For now this library is not working. IOTA changed encryption (Curl) and theses changes in this library are not implemented yet.


## Introduction

The Borlay.Iota.Library implements [[IOTA IRI api calls]](https://github.com/iotaledger/wiki/blob/master/api-proposal.md).
It also can do the proof of work for you.

http://iota.org
https://github.com/iotaledger

### Warning
This library is not fully functional yet. Please only use to create zero value messages.

Points to be aware of: 
	Bundle finalizing will likely fail.
	Transaction signing is untested at the moment.
	Not all the unit tests are updated yet.
	Code is not production ready and needs polishing.
	TimeStamps will likely fail Daylight saving tests.	
	
### Donate

```iota
PJGEZFOH99UVRUTRNWPZIYHHKWBMQKG9BHCMJGOZWWSPEWLTJYOHEFSJFQJHDBVOXLHMJATJIALSXELYXRBGAMWZHW
```

### TODO
	Polish code.
	Replace exceptions from API with a result object.
	Remove strings for values. i.e. transaction value is a long.
	Fix and test transaction signing + bundling.
	Make the code DRY
	Align the code more with SOLID principles.

### Technologies

The Borlay.Iota.Library is writen for [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) framework. It can be used in .net 4.5 and .net core 1.0 as well. For full .net frameworks support visit [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)


### Nuget

```PowerShell
Install-Package Borlay.Iota.Library
```
[Borlay.Iota.Library nuget](https://www.nuget.org/packages/Borlay.Iota.Library/)

### Getting started

Get the address with the balance and the transactions hashes
```cs
var api = new IotaApi("http://node.iotawallet.info:14265", 15);
var address = await api.GetAddress("YOURSEED", 0);

// use address
var balance = address.Balance;
var transactionHashes = address.Transactions;
```

 Renew your addresses
 ```cs
api.RenewBalances(address); // gets the balances
api.RenewTransactions(address); // gets the transactions hashes
api.RenewAddresses(address); // both
```

You can send empty transaction simply by doing this
```cs
var transfer = new TransferItem()
{
  Address = "ADDRESS",
  Value = 0,
  Message = "MESSAGETEST",
  Tag = "TAGTEST"
};
var transactionItem = await api.SendTransfer(transfer, CancellationToken.None);
```

Or you can send the transaction with the value
```cs
var transfer = new TransferItem()
{
  Address = "ADDRESS",
  Value = 1000,
  Message = "MESSAGETEST",
  Tag = "TAGTEST"
};
var transactionItem = await api.SendTransfer(transfer, "YOURSEED", 0, CancellationToken.None);
```
By default the pow will run on local pc but you can change to run it on iri
```cs
api.NonceSeeker = api.IriApi;
```

### POW

You can do the pow (attachToTangle) like this
```cs
var transactionTrytes = transfer.CreateTransactions().GetTrytes(); // gets transactions from transfer and then trytes
var toApprove = await api.IriApi.GetTransactionsToApprove(9); // gets transactions to approve
var trunk = toApprove.TrunkTransaction;
var branch = toApprove.BranchTransaction;

var trytesToSend = await transactionTrytes
                .DoPow(trunk, branch, 15, 0, CancellationToken.None); // do the pow
await api.BroadcastAndStore(trytesToSend); // broadcast and send the trytes
```
