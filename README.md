## Introduction

The BeeFrog.Iota.API implements [[IOTA IRI api calls]](https://github.com/iotaledger/wiki/blob/master/api-proposal.md).
It also can do the proof of work for you.
Currently supporting 1.3.0 of the API spec and working successfully with IRI 1.4.2.1

http://iota.org
https://github.com/iotaledger


### Donate

```iota
PJGEZFOH99UVRUTRNWPZIYHHKWBMQKG9BHCMJGOZWWSPEWLTJYOHEFSJFQJHDBVOXLHMJATJIALSXELYXRBGAMWZHW
```

### TODO
	Code Tidy up and restructure.
	Replace exceptions from API calls with a result object + return any errors.
	Add GPU Proof of Work. (This may take some time and may find end up in a separate library.)

### Technologies

The BeeFrog.Iota.API is written for [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard) framework. It can be used in .net 4.5 and .net core 1.0 as well. For full .net frameworks support visit [.NET Standard 1.1](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
Thanks to Borlay who create the base for which this API is built upon.


### Nuget

```PowerShell
Install-Package BeeFrog.Iota.API
```
[BeeFrog.Iota.API nuget](https://www.nuget.org/packages/BeeFrog.Iota.API/)

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

### More examples
Working examples can be found in the Examples folder. 
These include:
* Address Creation
* Sending a zero  value transaction.
* Sending multiple transactions in a bundle.
* Transaction promotion / re-attachment.