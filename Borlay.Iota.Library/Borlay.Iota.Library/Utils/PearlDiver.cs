#region Acknowledgements
/**
 * The code on this class is heavily based on:
 * 
 * See <https://github.com/iotaledger/iri/blob/dev/src/main/java/com/iota/iri/hash/PearlDiver.java
 * (c) 2016 Come-from-Beyond
 */
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Borlay.Iota.Library.Utils
{
    public class PearlDiver
    {
        enum State
        {
            RUNNING,
            CANCELLED,
            COMPLETED
        }

        const int TRANSACTION_LENGTH = 8019;

        const int CURL_HASH_LENGTH = 243;
        const int CURL_STATE_LENGTH = CURL_HASH_LENGTH * 3;
        private const int RequiredRounds = 81;
        ulong HIGH_BITS = 0b11111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111L;
        ulong LOW_BITS = 0b00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000L;

        volatile State state;

        Object interlock = new Object();

        public PearlDiver()
        {

        }
        public void Cancel()
        {
            lock (interlock)
            {
                state = State.CANCELLED;
            }
        }

        public async Task<string> DoPowAsync(string trytes, int minWeightMagnitude)
        {
            return DoPow(trytes, minWeightMagnitude);
        }

        public string DoPow(string trytes, int minWeightMagnitude)
        {
            var intxTrits = Library.Utils.Converter.ToTrits(trytes);
            Search(intxTrits, minWeightMagnitude).Wait();
            var resultTrytes = Utils.Converter.ToTrytes(intxTrits);
            return resultTrytes;
        }

        public async Task<bool> Search(int[] transactionTrits, int minWeightMagnitude)
        {
            if (transactionTrits.Length != TRANSACTION_LENGTH)
            {
                throw new ArgumentException("Invalid transaction trits length", "transactionTrits");
            }
            if (minWeightMagnitude < 0 || minWeightMagnitude > CURL_HASH_LENGTH)
            {
                throw new ArgumentException("Invalid min weight magnitude", "minWeightMagnitude");
            }

            lock(interlock)
            {
                state = State.RUNNING;
            }

            int numberOfProcs = 1; // Environment.ProcessorCount;

            ulong[] midCurlStateLow = new ulong[CURL_STATE_LENGTH];
            ulong[] midCurlStateHigh = new ulong[CURL_STATE_LENGTH];
            
            for (int i = CURL_HASH_LENGTH; i < CURL_STATE_LENGTH; i++)
            {
                midCurlStateLow[i] = HIGH_BITS;
                midCurlStateHigh[i] = HIGH_BITS;
            }

            int offset = 0;
            ulong[] curlScratchpadLow = new ulong[CURL_STATE_LENGTH];
            ulong[] curlScratchpadHigh = new ulong[CURL_STATE_LENGTH];
            for(int i = (TRANSACTION_LENGTH - CURL_HASH_LENGTH) / CURL_HASH_LENGTH; i-- > 0;)
            {
                for (int j = 0; j < CURL_HASH_LENGTH; j++)
                    {
                        switch (transactionTrits[offset++])
                        {
                            case 0: midCurlStateLow[j] = HIGH_BITS;
                                midCurlStateHigh[j] = HIGH_BITS;
                                break;

                            case 1: midCurlStateLow[j] = LOW_BITS;
                                midCurlStateHigh[j] = HIGH_BITS;
                                break;

                            default: midCurlStateLow[j] = HIGH_BITS;
                                midCurlStateHigh[j] = LOW_BITS;
                                break;
                        }
                    }

                Transform(midCurlStateLow, midCurlStateHigh, ref curlScratchpadLow, ref curlScratchpadHigh);
            }

            for (int i = 0; i < 162; i++)
                {

                    switch (transactionTrits[offset++])
                    {

                        case 0:
                            {

                                midCurlStateLow[i] = 0b1111111111111111111111111111111111111111111111111111111111111111L;
                                midCurlStateHigh[i] = 0b1111111111111111111111111111111111111111111111111111111111111111L;

                            }
                            break;

                        case 1:
                            {

                                midCurlStateLow[i] = 0b0000000000000000000000000000000000000000000000000000000000000000L;
                                midCurlStateHigh[i] = 0b1111111111111111111111111111111111111111111111111111111111111111L;

                            }
                            break;

                        default:
                            {

                                midCurlStateLow[i] = 0b1111111111111111111111111111111111111111111111111111111111111111L;
                                midCurlStateHigh[i] = 0b0000000000000000000000000000000000000000000000000000000000000000L;
                                break;
                            }
                    }
                }

            midCurlStateLow[162 + 0] = 0b1101101101101101101101101101101101101101101101101101101101101101L;
            midCurlStateHigh[162 + 0] = 0b1011011011011011011011011011011011011011011011011011011011011011L;
            midCurlStateLow[162 + 1] = 0b1111000111111000111111000111111000111111000111111000111111000111L;
            midCurlStateHigh[162 + 1] = 0b1000111111000111111000111111000111111000111111000111111000111111L;
            midCurlStateLow[162 + 2] = 0b0111111111111111111000000000111111111111111111000000000111111111L;
            midCurlStateHigh[162 + 2] = 0b1111111111000000000111111111111111111000000000111111111111111111L;
            midCurlStateLow[162 + 3] = 0b1111111111000000000000000000000000000111111111111111111111111111L;
            midCurlStateHigh[162 + 3] = 0b0000000000111111111111111111111111111111111111111111111111111111L;

            var tasks = new List<Task>();

            while (numberOfProcs-- > 0)
            {
                int threadIndex = numberOfProcs;
                var task = Task.Run(() =>
                {
                    ulong[] midCurlStateCopyLow = new ulong[CURL_STATE_LENGTH];
                    ulong[] midCurlStateCopyHigh = new ulong[CURL_STATE_LENGTH];
                    Array.Copy(midCurlStateLow, 0, midCurlStateCopyLow, 0, CURL_STATE_LENGTH);
                    Array.Copy(midCurlStateHigh, 0, midCurlStateCopyHigh, 0, CURL_STATE_LENGTH);
                    for (int i = threadIndex; i-- > 0;)
                    {
                        Increment(midCurlStateCopyLow, midCurlStateCopyHigh, 162 + CURL_HASH_LENGTH / 9, 162 + (CURL_HASH_LENGTH / 9) * 2);
                    }

                    ulong[] curlStateLow = new ulong[CURL_STATE_LENGTH];
                    ulong[] curlStateHigh = new ulong[CURL_STATE_LENGTH];
                    curlScratchpadLow = new ulong[CURL_STATE_LENGTH];
                    curlScratchpadHigh = new ulong[CURL_STATE_LENGTH];
                    ulong mask = 0;
                    ulong outMask = 1;

                    while (state == State.RUNNING && mask == 0)
                    {
                        Trace.WriteLine("Doing POW");

                        Increment(midCurlStateCopyLow, midCurlStateCopyHigh, 162 + (CURL_HASH_LENGTH / 9) * 2, CURL_HASH_LENGTH);

                        Array.Copy(midCurlStateCopyLow, 0, curlStateLow, 0, CURL_STATE_LENGTH);
                        Array.Copy(midCurlStateCopyHigh, 0, curlStateHigh, 0, CURL_STATE_LENGTH);
                        Transform(curlStateLow, curlStateHigh, ref curlScratchpadLow, ref curlScratchpadHigh);

                        mask = HIGH_BITS;
                        for (int i = minWeightMagnitude; i-- > 0;)
                        {
                            mask &= ~(curlStateLow[CURL_HASH_LENGTH - 1 - i] ^ curlStateHigh[CURL_HASH_LENGTH - 1 - i]);
                            if (mask == 0)
                            {
                                break;
                            }
                        }

                        if (mask == 0) continue;

                        lock (interlock)
                        {
                            if (state == State.RUNNING)
                            {
                                state = State.COMPLETED;
                                while ((outMask & mask) == 0)
                                {
                                    outMask <<= 1;
                                }
                                for (int i = 0; i < CURL_HASH_LENGTH; i++)
                                {
                                    transactionTrits[TRANSACTION_LENGTH - CURL_HASH_LENGTH + i] = (midCurlStateCopyLow[i] & outMask) == 0 ? 1 : (midCurlStateCopyHigh[i] & outMask) == 0 ? -1 : 0;
                                }
                            }
                        }
                    }
                });

                tasks.Add(task);
            }
            
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                lock (interlock)
                {
                    state = State.CANCELLED;
                }
            }            

            return state == State.COMPLETED;
        }
        
        private void Transform(ulong[] curlStateLow, ulong[] curlStateHigh, ref ulong[] curlScratchpadLow, ref ulong[] curlScratchpadHigh)
        {
            int curlScratchpadIndex = 0;
            for (int round = 0; round < RequiredRounds; round++)
            {
                Array.Copy(curlStateLow, 0, curlScratchpadLow, 0, CURL_STATE_LENGTH);
                Array.Copy(curlStateHigh, 0, curlScratchpadHigh, 0, CURL_STATE_LENGTH);

                for (int curlStateIndex = 0; curlStateIndex < CURL_STATE_LENGTH; curlStateIndex++)
                {
                    ulong alpha = curlScratchpadLow[curlScratchpadIndex];
                    ulong beta = curlScratchpadHigh[curlScratchpadIndex];
                    if (curlScratchpadIndex < 365)
                    {
                        curlScratchpadIndex += 364;
                    }
                    else
                    {
                        curlScratchpadIndex += -365;
                    }

                    ulong gamma = curlScratchpadHigh[curlScratchpadIndex];
                    ulong delta = (alpha | (~gamma)) & (curlScratchpadLow[curlScratchpadIndex] ^ beta);

                    curlStateLow[curlStateIndex] = ~delta;
                    curlStateHigh[curlStateIndex] = (alpha ^ gamma) | delta;
                }
            }
        }

        private void Increment(ulong[] midCurlStateCopyLow, ulong[] midCurlStateCopyHigh, int fromIndex, int toIndex)
        {

            for (int i = fromIndex; i < toIndex; i++)
            {
                if (midCurlStateCopyLow[i] == LOW_BITS)
                {
                    midCurlStateCopyLow[i] = HIGH_BITS;
                    midCurlStateCopyHigh[i] = LOW_BITS;
                }
                else
                {
                    if (midCurlStateCopyHigh[i] == LOW_BITS)
                    {
                        midCurlStateCopyHigh[i] = HIGH_BITS;
                    }
                    else
                    {
                        midCurlStateCopyLow[i] = LOW_BITS;
                    }
                    break;
                }
            }
        }
    }
}
