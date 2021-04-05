mergeInto(LibraryManager.library,
{ 
 
 // Create websocket and callback pointer
 SocketCreate: function (uri,hash,cb_open,cb_message,cb_close,cb_error) {
 
	if (window.sockets === undefined)
		window.sockets = {};
		
	window.sockets[hash] = new WebSocket(Pointer_stringify(uri));
	window.sockets[hash].binaryType = 'arraybuffer';

	// Call C# delegate
	window.sockets[hash].onopen = function(e) {
        Runtime.dynCall('vi', cb_open, [hash]);
	}
	
	window.sockets[hash].onerror = function(e){
		console.log("WebSocket.Error" + JSON.stringify(e));
		var size = lengthBytesUTF8(JSON.stringify(e))+1;
		var ptr_error = _malloc(size);
		stringToUTF8(window.sockets[hash].error,size-1);
		Runtime.dynCall('vii',cb_error,[hash,ptr_error]);
		_free(ptr_error);
	}
	
	window.sockets[hash].onmessage = function(e) {
		if (e.data instanceof Blob)
		{
			var reader = new FileReader();
			reader.addEventListener("loadend", function() {
				var array = new Uint8Array(reader.result);
				console.log("Recv : Blob " + array.length + " bytes");
				var ptr = _malloc(array.length);
				HEAPU8.set(array,ptr);
				Runtime.dynCall('viii',cb_message,[hash,ptr,array.length]);	// Call C# callback function
				_free(ptr);
			});
			reader.readAsArrayBuffer(e.data);
		}
		else if (e.data instanceof ArrayBuffer)
		{
			var array = new Uint8Array(e.data);
			console.log("Recv : ArrayBuffer " + array.length + " bytes");
			var ptr = _malloc(array.length);
			HEAPU8.set(array,ptr);
			Runtime.dynCall('viii',cb_message,[hash,ptr,array.length]); // Call C# callback function
			_free(ptr);
		}
	}
	
	window.sockets[hash].onclose = function(e) {

		if (e.code != 1000) {
			if (e.reason != null && e.reason.length > 0)
				window.sockets[hash].error = e.reason;
			else
				{
					switch (e.code)
					{
						case 1001: 
							window.sockets[hash].error = "Endpoint going away.";
							break;
						case 1002: 
							window.sockets[hash].error = "Protocol error.";
							break;
						case 1003: 
							window.sockets[hash].error = "Unsupported message.";
							break;
						case 1005: 
							window.sockets[hash].error = "No status.";
							break;
						case 1006: 
							window.sockets[hash].error = "Abnormal disconnection.";
							break;
						case 1009: 
							window.sockets[hash].error = "Data frame too large.";
							break;
						default:
							window.sockets[hash].error = "Error "+e.code;
							break;
					}
				}
		}
		var size = lengthBytesUTF8(window.sockets[hash].error)+1;
 		var ptr_error = _malloc(size);
		stringToUTF8(window.sockets[hash].error,ptr_error,size-1);
		Runtime.dynCall('viii',cb_close,[hash,e.code,ptr_error]);
		_free(ptr_error);
    }
	
 },

 SocketSend: function (hash, ptr, length)
 { 
	// Creating memory 
	if (window.sockets[hash].readyState === 1) 
	{
		var str_hex = '';
		// This is array buffer, must convert to Uint8Array
		var buffer = HEAPU8.buffer.slice(ptr,ptr + length);
		var byte_array = new Uint8Array(buffer);
		
		for(var i = 0; i < byte_array.length; i++)
			str_hex += byte_array[i].toString(16) + ' ';
			
		// console.log('array ' + byte_array.length + ' bytes : ' + str_hex);
		try 
		{
			window.sockets[hash].send (byte_array.buffer);
		}
		catch (e)
		{
			// Calling for close callback
			var size = lengthBytesUTF8(e)+1;
			var ptr_error = _malloc(size);
			stringToUTF8(window.sockets[hash].error,ptr_error,size-1);
			Runtime.dynCall('vii',cb_error,[hash,ptr_error]);
			_free(ptr_error);
			return false;
		}
		return true;
    }
	else
	{
		setTimeout(function () {SocketSend(hash, ptr, length);}, 1000);
		return false;
	}
	// console.log("JS.SocketSend " + length + " bytes");
	return false;
 },
 
 SocketState: function (hash){
	if (window.sockets[hash] === undefined || window.sockets[hash] === null)
		return 3;
	return window.sockets[hash].readyState;
 },

 SocketClose : function(hash){
	if (window.sockets[hash] === undefined || window.sockets[hash] === null)
		return;
	window.sockets[hash].close();
 }
 
});

