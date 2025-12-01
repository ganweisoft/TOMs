// index.ts

import { createPermissionDirective } from './permission';

const install = async function (app, packageId,routeName) {
    try {
        app.directive('permission', createPermissionDirective((packageId && routeName)?window[packageId][routeName]:[]));
    } catch (error) {
        console.error('Failed to initialize permissions:', error);
    }
}

export default {
    install
};
