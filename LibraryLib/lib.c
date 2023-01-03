#include "lib.h"
#include <string.h>
#include <stdlib.h>
#include <stdio.h>

Object* object;

Object* object_new(char* name, void* inner) {
	object = (Object*) malloc(sizeof(Object));
	object->name = malloc(50);
	strcpy_s(object->name, 50, name);
	object->inner = inner;

	return object;
}

char* object_get_name(Object* object) {
	return object->name;
}

void object_delete(Object* object) {
	if (object) {
		printf(object->name);
		free(object->name);
		free(object);
	}
}

void object_set_inner(Object *object, void *inner_obj) {
	object->inner = inner_obj;
}

void* object_get_inner(Object *object) {
	return object->inner;
}