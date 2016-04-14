#include "pch.h"
#include "Encryptor.h"
#include <Dpapi.h>
#include "Robuffer.h"

using namespace MASUtility;
using namespace Platform;
using namespace concurrency;
using namespace Microsoft::WRL;
using namespace Windows::Storage;

IAsyncOperation<IBuffer^>^ Encryptor::EncryptAsync(IBuffer^ data)
{
	return create_async([this, data]() -> IBuffer^
	{
		DATA_BLOB dataIn, dataOut;
		dataIn.pbData = GetDataFromIBuffer(data);
		dataIn.cbData = data->Length;

		if (!::CryptProtectData(&dataIn, L"", NULL, NULL, NULL, 0, &dataOut))
		{
			return nullptr;
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

IAsyncOperation<IBuffer^>^ Encryptor::DecryptAsync(IBuffer^ data)
{
	return create_async([this, data]() -> IBuffer^
	{
		DATA_BLOB dataIn, dataOut;
		dataIn.pbData = GetDataFromIBuffer(data);
		dataIn.cbData = data->Length;
		LPWSTR description = NULL;

		if (!::CryptUnprotectData(&dataIn, &description, NULL, NULL, NULL, 0, &dataOut))
		{
			return nullptr;
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
