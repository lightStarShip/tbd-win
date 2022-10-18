/* Code generated by cmd/cgo; DO NOT EDIT. */

/* package command-line-arguments */


#line 1 "cgo-builtin-export-prolog"

#include <stddef.h>

#ifndef GO_CGO_EXPORT_PROLOGUE_H
#define GO_CGO_EXPORT_PROLOGUE_H

#ifndef GO_CGO_GOSTRING_TYPEDEF
typedef struct { const char *p; ptrdiff_t n; } _GoString_;
#endif

#endif

/* Start of preamble from import "C" comments.  */


#line 3 "app.go"

#include "callback.h"

#line 1 "cgo-generated-wrapper"



/* End of preamble from import "C" comments.  */


/* Start of boilerplate cgo prologue.  */
#line 1 "cgo-gcc-export-header-prolog"

#ifndef GO_CGO_PROLOGUE_H
#define GO_CGO_PROLOGUE_H

typedef signed char GoInt8;
typedef unsigned char GoUint8;
typedef short GoInt16;
typedef unsigned short GoUint16;
typedef int GoInt32;
typedef unsigned int GoUint32;
typedef long long GoInt64;
typedef unsigned long long GoUint64;
typedef GoInt64 GoInt;
typedef GoUint64 GoUint;
typedef size_t GoUintptr;
typedef float GoFloat32;
typedef double GoFloat64;
#ifdef _MSC_VER
#include <complex.h>
typedef _Fcomplex GoComplex64;
typedef _Dcomplex GoComplex128;
#else
typedef float _Complex GoComplex64;
typedef double _Complex GoComplex128;
#endif

/*
  static assertion to make sure the file is being used on architecture
  at least with matching size of GoInt.
*/
typedef char _check_for_64_bit_pointer_matching_GoInt[sizeof(void*)==64/8 ? 1:-1];

#ifndef GO_CGO_GOSTRING_TYPEDEF
typedef _GoString_ GoString;
#endif
typedef void *GoMap;
typedef void *GoChan;
typedef struct { void *t; void *v; } GoInterface;
typedef struct { void *data; GoInt len; GoInt cap; } GoSlice;

#endif

/* End of boilerplate cgo prologue.  */

#ifdef __cplusplus
extern "C" {
#endif

extern __declspec(dllexport) void InitLib(GoInt8 isDebugPay, GoInt8 logLevel, GoString confUrl, UserInterfaceAPI cb, CallBackLog log);
extern __declspec(dllexport) char* StartProxy(GoString localProxyNetAddr, GoString nodeIP, GoString nodeWalletAddr);
extern __declspec(dllexport) void StopProxy();
extern __declspec(dllexport) GoUint8 ProxyStatus();
extern __declspec(dllexport) char* ChangeNode(GoString ip, GoString nodeAddr);
extern __declspec(dllexport) char* ScanQrCode(GoString path);
extern __declspec(dllexport) char* RuleVerInt();
extern __declspec(dllexport) char* RuleDataLoad();
extern __declspec(dllexport) char* ByPassDataLoad();
extern __declspec(dllexport) char* MustHitData();
extern __declspec(dllexport) char* NodeConfigData();
extern __declspec(dllexport) char* PriceConfigData();
extern __declspec(dllexport) GoInt32 MinerPort(GoString addr);
extern __declspec(dllexport) char* LoadCustomerBasic(GoString addr, GoString cusID);
extern __declspec(dllexport) GoFloat32 BalanceInDays(GoInt64 val);
extern __declspec(dllexport) char* TransferToFriend(GoString from, GoString to, GoInt days);
extern __declspec(dllexport) GoUint8 LibLoadWallet(GoString walletJSOn);
extern __declspec(dllexport) char* LibNewWallet(GoString auth);
extern __declspec(dllexport) GoUint8 LibImportWallet(GoString walletJSOn, GoString auth);
extern __declspec(dllexport) GoUint8 LibIsOpen();
extern __declspec(dllexport) GoUint8 LibOpenWallet(GoString auth);
extern __declspec(dllexport) GoFloat32 LibGetPingVal(GoString mid, GoString minerIP);

#ifdef __cplusplus
}
#endif
