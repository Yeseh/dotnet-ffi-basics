typedef struct Object {
	char* name;
	// User provided opaque object, managed by C#
	void* inner;
} Object;

__declspec(dllexport)
Object* object_new(char*, void*);

__declspec(dllexport)
char* object_get_name(Object*);

__declspec(dllexport)
void object_delete(Object*);

__declspec(dllexport)
void object_set_inner(Object*, void*);

__declspec(dllexport)
void* object_get_inner(Object*);

