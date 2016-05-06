#include "pch.h"
#include "Encryptor.h"
#include <Dpapi.h>
#include "Robuffer.h"

using namespace MASUtility;
using namespace Platform;
using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Windows::Storage;

IAsyncOperation<IBuffer^>^ Encryptor::EncryptAsync(IBuffer^ data, String^ entropy)
{
	return create_async([this, data, entropy]() -> IBuffer^
	{
		DATA_BLOB dataIn, dataOut;
		dataIn.pbData = GetDataFromIBuffer(data);
		dataIn.cbData = data->Length;

		DATA_BLOB* pEntropyData = NULL;
		DATA_BLOB entropyData;
		if (entropy != nullptr)
		{
			entropyData.pbData = (BYTE*)entropy->Data();
			entropyData.cbData = entropy->Length() * sizeof(wchar_t);
			pEntropyData = &entropyData;
		}

		if (!::CryptProtectData(&dataIn, L"", pEntropyData, NULL, NULL, 0, &dataOut))
		{
			DWORD dwError = ::GetLastError();
			throw ref new COMException(HRESULT_FROM_WIN32(dwError));
		}
		else
		{
			DataWriter^ writer = ref new DataWriter();

			Array<BYTE>^ array = ref new Array<BYTE>(dataOut.pbData, dataOut.cbData);
			writer->WriteBytes(array);
			return writer->DetachBuffer();
		}
	});
}

IAsyncOperation<IBuffer^>^ Encryptor::DecryptAsync(IBuffer^ data, String^ entropy)
{
	return create_async([this, data, entropy]() -> IBuffer^
	{
		DATA_BLOB dataIn, dataOut;
		dataIn.pbData = GetDataFromIBuffer(data);
		dataIn.cbData = data->Length;
		LPWSTR description = NULL;

		DATA_BLOB* pEntropyData = NULL;
		DATA_BLOB entropyData;
		if (entropy != nullptr)
		{
			entropyData.pbData = (BYTE*)entropy->Data();
			entropyData.cbData = entropy->Length() * sizeof(wchar_t);
			pEntropyData = &entropyData;
		}

		if (!::CryptUnprotectData(&dataIn, &description, pEntropyData, NULL, NULL, 0, &dataOut))
		{
			DWORD dwError = ::GetLastError();
			throw ref new COMException(HRESULT_FROM_WIN32(dwError));
		}
		else
		{
			DataWriter^ writer = ref new DataWriter();

			Array<BYTE>^ array = ref new Array<BYTE>(dataOut.pbData, dataOut.cbData);
			writer->WriteBytes(array);
			return writer->DetachBuffer();
		}
	});
}

BYTE* Encryptor::GetDataFromIBuffer(IBuffer^ buffer)
{
	Object^ obj = buffer;
	ComPtr<IInspectable> insp(reinterpret_cast<IInspectable*>(obj));

	ComPtr<IBufferByteAccess> bufferByteAccess;
	HRESULT hr = insp.As(&bufferByteAccess);

	// Retrieve the buffer data.

	BYTE* bufferData = nullptr;
	if (SUCCEEDED(hr))
	{
		hr = bufferByteAccess->Buffer(&bufferData);
	}

	return bufferData;
}
