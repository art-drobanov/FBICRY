
#include "stdafx.h"
#include "Keccak-org.h"

#pragma warning(disable : 4244)

namespace KeccakOrg
{
typedef unsigned char UINT8;
typedef unsigned long long int UINT64;

#define ROL64(a, offset) _rotl64(a, offset)

const UINT64 KeccakF1600RoundConstants[24] = {
    0x0000000000000001ULL,
    0x0000000000008082ULL,
    0x800000000000808aULL,
    0x8000000080008000ULL,
    0x000000000000808bULL,
    0x0000000080000001ULL,
    0x8000000080008081ULL,
    0x8000000000008009ULL,
    0x000000000000008aULL,
    0x0000000000000088ULL,
    0x0000000080008009ULL,
    0x000000008000000aULL,
    0x000000008000808bULL,
    0x800000000000008bULL,
    0x8000000000008089ULL,
    0x8000000000008003ULL,
    0x8000000000008002ULL,
    0x8000000000000080ULL,
    0x000000000000800aULL,
    0x800000008000000aULL,
    0x8000000080008081ULL,
    0x8000000000008080ULL,
    0x0000000080000001ULL,
    0x8000000080008008ULL };


#define declareABCDE \
    UINT64 Aba, Abe, Abi, Abo, Abu; \
    UINT64 Aga, Age, Agi, Ago, Agu; \
    UINT64 Aka, Ake, Aki, Ako, Aku; \
    UINT64 Ama, Ame, Ami, Amo, Amu; \
    UINT64 Asa, Ase, Asi, Aso, Asu; \
    UINT64 Bba, Bbe, Bbi, Bbo, Bbu; \
    UINT64 Bga, Bge, Bgi, Bgo, Bgu; \
    UINT64 Bka, Bke, Bki, Bko, Bku; \
    UINT64 Bma, Bme, Bmi, Bmo, Bmu; \
    UINT64 Bsa, Bse, Bsi, Bso, Bsu; \
    UINT64 Ca, Ce, Ci, Co, Cu; \
    UINT64 Da, De, Di, Do, Du; \
    UINT64 Eba, Ebe, Ebi, Ebo, Ebu; \
    UINT64 Ega, Ege, Egi, Ego, Egu; \
    UINT64 Eka, Eke, Eki, Eko, Eku; \
    UINT64 Ema, Eme, Emi, Emo, Emu; \
    UINT64 Esa, Ese, Esi, Eso, Esu; \

#define prepareTheta \
    Ca = Aba^Aga^Aka^Ama^Asa; \
    Ce = Abe^Age^Ake^Ame^Ase; \
    Ci = Abi^Agi^Aki^Ami^Asi; \
    Co = Abo^Ago^Ako^Amo^Aso; \
    Cu = Abu^Agu^Aku^Amu^Asu; \

// --- Theta Rho Pi Chi Iota Prepare-theta (lane complementing pattern 'bebigokimisa')
// --- 64-bit lanes mapped to 64-bit words
#define thetaRhoPiChiIotaPrepareTheta(i, A, E) \
    Da = Cu^ROL64(Ce, 1); \
    De = Ca^ROL64(Ci, 1); \
    Di = Ce^ROL64(Co, 1); \
    Do = Ci^ROL64(Cu, 1); \
    Du = Co^ROL64(Ca, 1); \
\
    A##ba ^= Da; \
    Bba = A##ba; \
    A##ge ^= De; \
    Bbe = ROL64(A##ge, 44); \
    A##ki ^= Di; \
    Bbi = ROL64(A##ki, 43); \
    E##ba =   Bba ^(  Bbe |  Bbi ); \
    E##ba ^= KeccakF1600RoundConstants[i]; \
    Ca = E##ba; \
    A##mo ^= Do; \
    Bbo = ROL64(A##mo, 21); \
    E##be =   Bbe ^((~Bbi)|  Bbo ); \
    Ce = E##be; \
    A##su ^= Du; \
    Bbu = ROL64(A##su, 14); \
    E##bi =   Bbi ^(  Bbo &  Bbu ); \
    Ci = E##bi; \
    E##bo =   Bbo ^(  Bbu |  Bba ); \
    Co = E##bo; \
    E##bu =   Bbu ^(  Bba &  Bbe ); \
    Cu = E##bu; \
\
    A##bo ^= Do; \
    Bga = ROL64(A##bo, 28); \
    A##gu ^= Du; \
    Bge = ROL64(A##gu, 20); \
    A##ka ^= Da; \
    Bgi = ROL64(A##ka, 3); \
    E##ga =   Bga ^(  Bge |  Bgi ); \
    Ca ^= E##ga; \
    A##me ^= De; \
    Bgo = ROL64(A##me, 45); \
    E##ge =   Bge ^(  Bgi &  Bgo ); \
    Ce ^= E##ge; \
    A##si ^= Di; \
    Bgu = ROL64(A##si, 61); \
    E##gi =   Bgi ^(  Bgo |(~Bgu)); \
    Ci ^= E##gi; \
    E##go =   Bgo ^(  Bgu |  Bga ); \
    Co ^= E##go; \
    E##gu =   Bgu ^(  Bga &  Bge ); \
    Cu ^= E##gu; \
\
    A##be ^= De; \
    Bka = ROL64(A##be, 1); \
    A##gi ^= Di; \
    Bke = ROL64(A##gi, 6); \
    A##ko ^= Do; \
    Bki = ROL64(A##ko, 25); \
    E##ka =   Bka ^(  Bke |  Bki ); \
    Ca ^= E##ka; \
    A##mu ^= Du; \
    Bko = ROL64(A##mu, 8); \
    E##ke =   Bke ^(  Bki &  Bko ); \
    Ce ^= E##ke; \
    A##sa ^= Da; \
    Bku = ROL64(A##sa, 18); \
    E##ki =   Bki ^((~Bko)&  Bku ); \
    Ci ^= E##ki; \
    E##ko = (~Bko)^(  Bku |  Bka ); \
    Co ^= E##ko; \
    E##ku =   Bku ^(  Bka &  Bke ); \
    Cu ^= E##ku; \
\
    A##bu ^= Du; \
    Bma = ROL64(A##bu, 27); \
    A##ga ^= Da; \
    Bme = ROL64(A##ga, 36); \
    A##ke ^= De; \
    Bmi = ROL64(A##ke, 10); \
    E##ma =   Bma ^(  Bme &  Bmi ); \
    Ca ^= E##ma; \
    A##mi ^= Di; \
    Bmo = ROL64(A##mi, 15); \
    E##me =   Bme ^(  Bmi |  Bmo ); \
    Ce ^= E##me; \
    A##so ^= Do; \
    Bmu = ROL64(A##so, 56); \
    E##mi =   Bmi ^((~Bmo)|  Bmu ); \
    Ci ^= E##mi; \
    E##mo = (~Bmo)^(  Bmu &  Bma ); \
    Co ^= E##mo; \
    E##mu =   Bmu ^(  Bma |  Bme ); \
    Cu ^= E##mu; \
\
    A##bi ^= Di; \
    Bsa = ROL64(A##bi, 62); \
    A##go ^= Do; \
    Bse = ROL64(A##go, 55); \
    A##ku ^= Du; \
    Bsi = ROL64(A##ku, 39); \
    E##sa =   Bsa ^((~Bse)&  Bsi ); \
    Ca ^= E##sa; \
    A##ma ^= Da; \
    Bso = ROL64(A##ma, 41); \
    E##se = (~Bse)^(  Bsi |  Bso ); \
    Ce ^= E##se; \
    A##se ^= De; \
    Bsu = ROL64(A##se, 2); \
    E##si =   Bsi ^(  Bso &  Bsu ); \
    Ci ^= E##si; \
    E##so =   Bso ^(  Bsu |  Bsa ); \
    Co ^= E##so; \
    E##su =   Bsu ^(  Bsa &  Bse ); \
    Cu ^= E##su; \
\

// --- Theta Rho Pi Chi Iota (lane complementing pattern 'bebigokimisa')
// --- 64-bit lanes mapped to 64-bit words
#define thetaRhoPiChiIota(i, A, E) \
    Da = Cu^ROL64(Ce, 1); \
    De = Ca^ROL64(Ci, 1); \
    Di = Ce^ROL64(Co, 1); \
    Do = Ci^ROL64(Cu, 1); \
    Du = Co^ROL64(Ca, 1); \
\
    A##ba ^= Da; \
    Bba = A##ba; \
    A##ge ^= De; \
    Bbe = ROL64(A##ge, 44); \
    A##ki ^= Di; \
    Bbi = ROL64(A##ki, 43); \
    E##ba =   Bba ^(  Bbe |  Bbi ); \
    E##ba ^= KeccakF1600RoundConstants[i]; \
    A##mo ^= Do; \
    Bbo = ROL64(A##mo, 21); \
    E##be =   Bbe ^((~Bbi)|  Bbo ); \
    A##su ^= Du; \
    Bbu = ROL64(A##su, 14); \
    E##bi =   Bbi ^(  Bbo &  Bbu ); \
    E##bo =   Bbo ^(  Bbu |  Bba ); \
    E##bu =   Bbu ^(  Bba &  Bbe ); \
\
    A##bo ^= Do; \
    Bga = ROL64(A##bo, 28); \
    A##gu ^= Du; \
    Bge = ROL64(A##gu, 20); \
    A##ka ^= Da; \
    Bgi = ROL64(A##ka, 3); \
    E##ga =   Bga ^(  Bge |  Bgi ); \
    A##me ^= De; \
    Bgo = ROL64(A##me, 45); \
    E##ge =   Bge ^(  Bgi &  Bgo ); \
    A##si ^= Di; \
    Bgu = ROL64(A##si, 61); \
    E##gi =   Bgi ^(  Bgo |(~Bgu)); \
    E##go =   Bgo ^(  Bgu |  Bga ); \
    E##gu =   Bgu ^(  Bga &  Bge ); \
\
    A##be ^= De; \
    Bka = ROL64(A##be, 1); \
    A##gi ^= Di; \
    Bke = ROL64(A##gi, 6); \
    A##ko ^= Do; \
    Bki = ROL64(A##ko, 25); \
    E##ka =   Bka ^(  Bke |  Bki ); \
    A##mu ^= Du; \
    Bko = ROL64(A##mu, 8); \
    E##ke =   Bke ^(  Bki &  Bko ); \
    A##sa ^= Da; \
    Bku = ROL64(A##sa, 18); \
    E##ki =   Bki ^((~Bko)&  Bku ); \
    E##ko = (~Bko)^(  Bku |  Bka ); \
    E##ku =   Bku ^(  Bka &  Bke ); \
\
    A##bu ^= Du; \
    Bma = ROL64(A##bu, 27); \
    A##ga ^= Da; \
    Bme = ROL64(A##ga, 36); \
    A##ke ^= De; \
    Bmi = ROL64(A##ke, 10); \
    E##ma =   Bma ^(  Bme &  Bmi ); \
    A##mi ^= Di; \
    Bmo = ROL64(A##mi, 15); \
    E##me =   Bme ^(  Bmi |  Bmo ); \
    A##so ^= Do; \
    Bmu = ROL64(A##so, 56); \
    E##mi =   Bmi ^((~Bmo)|  Bmu ); \
    E##mo = (~Bmo)^(  Bmu &  Bma ); \
    E##mu =   Bmu ^(  Bma |  Bme ); \
\
    A##bi ^= Di; \
    Bsa = ROL64(A##bi, 62); \
    A##go ^= Do; \
    Bse = ROL64(A##go, 55); \
    A##ku ^= Du; \
    Bsi = ROL64(A##ku, 39); \
    E##sa =   Bsa ^((~Bse)&  Bsi ); \
    A##ma ^= Da; \
    Bso = ROL64(A##ma, 41); \
    E##se = (~Bse)^(  Bsi |  Bso ); \
    A##se ^= De; \
    Bsu = ROL64(A##se, 2); \
    E##si =   Bsi ^(  Bso &  Bsu ); \
    E##so =   Bso ^(  Bsu |  Bsa ); \
    E##su =   Bsu ^(  Bsa &  Bse ); \
\

#define copyFromStateAndXor1024bits(X, state, input) \
    X##ba = state[ 0]^input[ 0]; \
    X##be = state[ 1]^input[ 1]; \
    X##bi = state[ 2]^input[ 2]; \
    X##bo = state[ 3]^input[ 3]; \
    X##bu = state[ 4]^input[ 4]; \
    X##ga = state[ 5]^input[ 5]; \
    X##ge = state[ 6]^input[ 6]; \
    X##gi = state[ 7]^input[ 7]; \
    X##go = state[ 8]^input[ 8]; \
    X##gu = state[ 9]^input[ 9]; \
    X##ka = state[10]^input[10]; \
    X##ke = state[11]^input[11]; \
    X##ki = state[12]^input[12]; \
    X##ko = state[13]^input[13]; \
    X##ku = state[14]^input[14]; \
    X##ma = state[15]^input[15]; \
    X##me = state[16]; \
    X##mi = state[17]; \
    X##mo = state[18]; \
    X##mu = state[19]; \
    X##sa = state[20]; \
    X##se = state[21]; \
    X##si = state[22]; \
    X##so = state[23]; \
    X##su = state[24]; \

#define copyFromStateAndXor1088bits(X, state, input) \
    X##ba = state[ 0]^input[ 0]; \
    X##be = state[ 1]^input[ 1]; \
    X##bi = state[ 2]^input[ 2]; \
    X##bo = state[ 3]^input[ 3]; \
    X##bu = state[ 4]^input[ 4]; \
    X##ga = state[ 5]^input[ 5]; \
    X##ge = state[ 6]^input[ 6]; \
    X##gi = state[ 7]^input[ 7]; \
    X##go = state[ 8]^input[ 8]; \
    X##gu = state[ 9]^input[ 9]; \
    X##ka = state[10]^input[10]; \
    X##ke = state[11]^input[11]; \
    X##ki = state[12]^input[12]; \
    X##ko = state[13]^input[13]; \
    X##ku = state[14]^input[14]; \
    X##ma = state[15]^input[15]; \
    X##me = state[16]^input[16]; \
    X##mi = state[17]; \
    X##mo = state[18]; \
    X##mu = state[19]; \
    X##sa = state[20]; \
    X##se = state[21]; \
    X##si = state[22]; \
    X##so = state[23]; \
    X##su = state[24]; \

#define copyFromState(X, state) \
    X##ba = state[ 0]; \
    X##be = state[ 1]; \
    X##bi = state[ 2]; \
    X##bo = state[ 3]; \
    X##bu = state[ 4]; \
    X##ga = state[ 5]; \
    X##ge = state[ 6]; \
    X##gi = state[ 7]; \
    X##go = state[ 8]; \
    X##gu = state[ 9]; \
    X##ka = state[10]; \
    X##ke = state[11]; \
    X##ki = state[12]; \
    X##ko = state[13]; \
    X##ku = state[14]; \
    X##ma = state[15]; \
    X##me = state[16]; \
    X##mi = state[17]; \
    X##mo = state[18]; \
    X##mu = state[19]; \
    X##sa = state[20]; \
    X##se = state[21]; \
    X##si = state[22]; \
    X##so = state[23]; \
    X##su = state[24]; \

#define copyToState(state, X) \
    state[ 0] = X##ba; \
    state[ 1] = X##be; \
    state[ 2] = X##bi; \
    state[ 3] = X##bo; \
    state[ 4] = X##bu; \
    state[ 5] = X##ga; \
    state[ 6] = X##ge; \
    state[ 7] = X##gi; \
    state[ 8] = X##go; \
    state[ 9] = X##gu; \
    state[10] = X##ka; \
    state[11] = X##ke; \
    state[12] = X##ki; \
    state[13] = X##ko; \
    state[14] = X##ku; \
    state[15] = X##ma; \
    state[16] = X##me; \
    state[17] = X##mi; \
    state[18] = X##mo; \
    state[19] = X##mu; \
    state[20] = X##sa; \
    state[21] = X##se; \
    state[22] = X##si; \
    state[23] = X##so; \
    state[24] = X##su; \

#define copyStateVariables(X, Y) \
    X##ba = Y##ba; \
    X##be = Y##be; \
    X##bi = Y##bi; \
    X##bo = Y##bo; \
    X##bu = Y##bu; \
    X##ga = Y##ga; \
    X##ge = Y##ge; \
    X##gi = Y##gi; \
    X##go = Y##go; \
    X##gu = Y##gu; \
    X##ka = Y##ka; \
    X##ke = Y##ke; \
    X##ki = Y##ki; \
    X##ko = Y##ko; \
    X##ku = Y##ku; \
    X##ma = Y##ma; \
    X##me = Y##me; \
    X##mi = Y##mi; \
    X##mo = Y##mo; \
    X##mu = Y##mu; \
    X##sa = Y##sa; \
    X##se = Y##se; \
    X##si = Y##si; \
    X##so = Y##so; \
    X##su = Y##su; \

#define rounds \
    prepareTheta \
    thetaRhoPiChiIotaPrepareTheta( 0, A, E) \
    thetaRhoPiChiIotaPrepareTheta( 1, E, A) \
    thetaRhoPiChiIotaPrepareTheta( 2, A, E) \
    thetaRhoPiChiIotaPrepareTheta( 3, E, A) \
    thetaRhoPiChiIotaPrepareTheta( 4, A, E) \
    thetaRhoPiChiIotaPrepareTheta( 5, E, A) \
    thetaRhoPiChiIotaPrepareTheta( 6, A, E) \
    thetaRhoPiChiIotaPrepareTheta( 7, E, A) \
    thetaRhoPiChiIotaPrepareTheta( 8, A, E) \
    thetaRhoPiChiIotaPrepareTheta( 9, E, A) \
    thetaRhoPiChiIotaPrepareTheta(10, A, E) \
    thetaRhoPiChiIotaPrepareTheta(11, E, A) \
    thetaRhoPiChiIotaPrepareTheta(12, A, E) \
    thetaRhoPiChiIotaPrepareTheta(13, E, A) \
    thetaRhoPiChiIotaPrepareTheta(14, A, E) \
    thetaRhoPiChiIotaPrepareTheta(15, E, A) \
    thetaRhoPiChiIotaPrepareTheta(16, A, E) \
    thetaRhoPiChiIotaPrepareTheta(17, E, A) \
    thetaRhoPiChiIotaPrepareTheta(18, A, E) \
    thetaRhoPiChiIotaPrepareTheta(19, E, A) \
    thetaRhoPiChiIotaPrepareTheta(20, A, E) \
    thetaRhoPiChiIotaPrepareTheta(21, E, A) \
    thetaRhoPiChiIotaPrepareTheta(22, A, E) \
    thetaRhoPiChiIota(23, E, A) \
    copyToState(state, A)

void KeccakPermutationOnWords(UINT64 *state)
{
    declareABCDE

    copyFromState(A, state)
    rounds
}

void KeccakPermutationOnWordsAfterXoring(UINT64 *state, const UINT64 *input, unsigned int laneCount)
{
    declareABCDE
	unsigned int j;

    for(j=0; j<laneCount; j++)
        state[j] ^= input[j];	
    copyFromState(A, state)
    rounds
}

void KeccakPermutationOnWordsAfterXoring1024bits(UINT64 *state, const UINT64 *input)
{
    declareABCDE

    copyFromStateAndXor1024bits(A, state, input)
    rounds
}

void KeccakPermutationOnWordsAfterXoring1088bits(UINT64 *state, const UINT64 *input)
{
    declareABCDE

    copyFromStateAndXor1088bits(A, state, input)
    rounds
}

void KeccakInitialize()
{
}

void KeccakInitializeState(unsigned char *state)
{
    memset(state, 0, KeccakPermutationSizeInBytes);
    ((UINT64*)state)[ 1] = ~(UINT64)0;
    ((UINT64*)state)[ 2] = ~(UINT64)0;
    ((UINT64*)state)[ 8] = ~(UINT64)0;
    ((UINT64*)state)[12] = ~(UINT64)0;
    ((UINT64*)state)[17] = ~(UINT64)0;
    ((UINT64*)state)[20] = ~(UINT64)0;
}

void KeccakPermutation(unsigned char *state)
{
    // We assume the state is always stored as words
    KeccakPermutationOnWords((UINT64*)state);
}

void fromBytesToWord(UINT64 *word, const UINT8 *bytes)
{
    unsigned int i;

    *word = 0;
    for(i=0; i<(64/8); i++)
        *word |= (UINT64)(bytes[i]) << (8*i);
}

void KeccakAbsorb1024bits(unsigned char *state, const unsigned char *data)
{
    KeccakPermutationOnWordsAfterXoring1024bits((UINT64*)state, (const UINT64*)data);
}

void KeccakAbsorb1088bits(unsigned char *state, const unsigned char *data)
{
    KeccakPermutationOnWordsAfterXoring1088bits((UINT64*)state, (const UINT64*)data);
}

void KeccakAbsorb(unsigned char *state, const unsigned char *data, unsigned int laneCount)
{
    KeccakPermutationOnWordsAfterXoring((UINT64*)state, (const UINT64*)data, laneCount);
}

void fromWordToBytes(UINT8 *bytes, const UINT64 word)
{
    unsigned int i;

    for(i=0; i<(64/8); i++)
        bytes[i] = (word >> (8*i)) & 0xFF;
}

void KeccakExtract1024bits(const unsigned char *state, unsigned char *data)
{
    memcpy(data, state, 128);

    ((UINT64*)data)[ 1] = ~((UINT64*)data)[ 1];
    ((UINT64*)data)[ 2] = ~((UINT64*)data)[ 2];
    ((UINT64*)data)[ 8] = ~((UINT64*)data)[ 8];
    ((UINT64*)data)[12] = ~((UINT64*)data)[12];
}

void KeccakExtract(const unsigned char *state, unsigned char *data, unsigned int laneCount)
{
    memcpy(data, state, laneCount*8);

    if (laneCount > 1) {
        ((UINT64*)data)[ 1] = ~((UINT64*)data)[ 1];
        if (laneCount > 2) {
            ((UINT64*)data)[ 2] = ~((UINT64*)data)[ 2];
            if (laneCount > 8) {
                ((UINT64*)data)[ 8] = ~((UINT64*)data)[ 8];
                if (laneCount > 12) {
                    ((UINT64*)data)[12] = ~((UINT64*)data)[12];
                }
            }
        }
    }
}

HashReturn Init(hashState *state, int hashbitlen)
{
    KeccakInitialize();
    switch(hashbitlen) {
        case 0: // Arbitrary length output
            state->capacity = 576;
            break;
        case 224:
            state->capacity = 448;
            break;
        case 256:
            state->capacity = 512;
            break;
        case 384:
            state->capacity = 768;
            break;
        case 512:
            state->capacity = 1024;
            break;
        default:
            return BAD_HASHLEN;
    }
    state->rate = KeccakPermutationSize - state->capacity;
    state->diversifier = hashbitlen/8;
    state->hashbitlen = hashbitlen;
    KeccakInitializeState(state->state);
    memset(state->dataQueue, 0, KeccakMaximumRateInBytes);
    state->bitsInQueue = 0;
    state->squeezing = 0;
    state->bitsAvailableForSqueezing = 0;

    return SUCCESS;
}

void AbsorbQueue(hashState *state)
{
    // state->bitsInQueue is assumed to be equal a multiple of 8
    memset(state->dataQueue+state->bitsInQueue/8, 0, state->rate/8-state->bitsInQueue/8);
    if (state->rate == 1024)
        KeccakAbsorb1024bits(state->state, state->dataQueue);
    else if (state->rate == 1088)
        KeccakAbsorb1088bits(state->state, state->dataQueue);
    else
        KeccakAbsorb(state->state, state->dataQueue, state->rate/64);
    state->bitsInQueue = 0;
}

HashReturn Update(hashState *state, const BitSequence *data, DataLength databitlen)
{
    DataLength i, j;
    DataLength partialBlock, partialByte, wholeBlocks;
    BitSequence lastByte;
    const BitSequence *curData;

    if ((state->bitsInQueue % 8) != 0)
        return FAIL; // Only the last call may contain a partial byte
    if (state->squeezing)
        return FAIL; // Too late for additional input

    i = 0;
    while(i < databitlen) {
        if ((state->bitsInQueue == 0) && (databitlen >= state->rate) && (i <= (databitlen-state->rate))) {
            wholeBlocks = (databitlen-i)/state->rate;
            curData = data+i/8;
            if (state->rate == 1024) {
                for(j=0; j<wholeBlocks; j++, curData+=1024/8) {
                    KeccakAbsorb1024bits(state->state, curData);
                }
            }
            else if (state->rate == 1088) {
                for(j=0; j<wholeBlocks; j++, curData+=1088/8) {
                    KeccakAbsorb1088bits(state->state, curData);
                }
            }
            else {
                for(j=0; j<wholeBlocks; j++, curData+=state->rate/8) {
                    KeccakAbsorb(state->state, curData, state->rate/64);
                }
            }
            i += wholeBlocks*state->rate;
        }
        else {
            partialBlock = databitlen - i;
            if (partialBlock+state->bitsInQueue > state->rate)
                partialBlock = state->rate-state->bitsInQueue;
            partialByte = partialBlock % 8;
            partialBlock -= partialByte;
            memcpy(state->dataQueue+state->bitsInQueue/8, data+i/8, partialBlock/8);
            state->bitsInQueue += partialBlock;
            i += partialBlock;
            if (state->bitsInQueue == state->rate)
                AbsorbQueue(state);
            if (partialByte > 0) {
                // Align the last partial byte to the least significant bits
                lastByte = data[i/8] >> (8-partialByte);
                state->dataQueue[state->bitsInQueue/8] = lastByte;
                state->bitsInQueue += partialByte;
                i += partialByte;
            }
        }
    }
    return SUCCESS;
}

void PadAndSwitchToSqueezingPhase(hashState *state)
{
    if ((state->bitsInQueue % 8) != 0) {
        // The bits are numbered from 0=LSB to 7=MSB
        unsigned char padByte = 1 << (state->bitsInQueue % 8);
        state->dataQueue[state->bitsInQueue/8] |= padByte;
        state->bitsInQueue += 8-(state->bitsInQueue % 8);
    }
    else {
        state->dataQueue[state->bitsInQueue/8] = 0x01;
        state->bitsInQueue += 8;
    }
    if (state->bitsInQueue == state->rate)
        AbsorbQueue(state);
    state->dataQueue[state->bitsInQueue/8] = state->diversifier;
    state->bitsInQueue += 8;
    if (state->bitsInQueue == state->rate)
        AbsorbQueue(state);
    state->dataQueue[state->bitsInQueue/8] = state->rate/8;
    state->bitsInQueue += 8;
    if (state->bitsInQueue == state->rate)
        AbsorbQueue(state);
    state->dataQueue[state->bitsInQueue/8] = 0x01;
    state->bitsInQueue += 8;
    if (state->bitsInQueue > 0)
        AbsorbQueue(state);
    if ((state->rate == 1024) && ((state->hashbitlen > 512) || (state->hashbitlen == 0))) {
        KeccakExtract1024bits(state->state, state->dataQueue);
        state->bitsAvailableForSqueezing = 1024;
    }
    else {
        KeccakExtract(state->state, state->dataQueue, state->rate/64);
        state->bitsAvailableForSqueezing = state->rate;
    }
    state->squeezing = 1;
}

HashReturn Final(hashState *state, BitSequence *hashval)
{
    if (state->squeezing)
        return FAIL; // Too late, we are already squeezing
    PadAndSwitchToSqueezingPhase(state);
    if (state->hashbitlen > 0)
        memcpy(hashval, state->dataQueue, state->hashbitlen/8);
    return SUCCESS;
}

HashReturn Squeeze(hashState *state, BitSequence *output, DataLength outputLength)
{
    DataLength i;
    DataLength partialBlock;

    if (!state->squeezing)
        return FAIL; // Too early, we are still absorbing
    if (state->hashbitlen != 0)
        return FAIL; // Arbitrary length output is not permitted in this case
    if ((outputLength % 8) != 0)
        return FAIL; // Only multiple of 8 bits are allowed, truncation can be done at user level

    i = 0;
    while(i < outputLength) {
        if (state->bitsAvailableForSqueezing == 0) {
            KeccakPermutation(state->state);
            if (state->rate == 1024) {
                KeccakExtract1024bits(state->state, state->dataQueue);
                state->bitsAvailableForSqueezing = state->rate;
            }
            else
                return FAIL; // Inconsistent rate
        }
        partialBlock = outputLength - i;
        if (partialBlock > state->bitsAvailableForSqueezing)
            partialBlock = state->bitsAvailableForSqueezing;
        memcpy(output+i/8, state->dataQueue+(state->rate-state->bitsAvailableForSqueezing)/8, partialBlock/8);
        state->bitsAvailableForSqueezing -= partialBlock;
        i += partialBlock;
    }
    return SUCCESS;
}

HashReturn Hash(int hashbitlen, const BitSequence *data, DataLength databitlen, BitSequence *hashval)
{
    hashState state;
    HashReturn result;

    if (hashbitlen == 0)
        return BAD_HASHLEN; // Arbitrary length output not available through this API
    result = Init(&state, hashbitlen);
    if (result != SUCCESS)
        return result;
    result = Update(&state, data, databitlen);
    if (result != SUCCESS)
        return result;
    result = Final(&state, hashval);
    return result;
}

}