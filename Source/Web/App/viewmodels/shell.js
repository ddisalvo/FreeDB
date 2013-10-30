define(['durandal/system', 'plugins/router', 'services/logger'],
    function (system, router, logger) {
        var shell = {
            activate: activate,
            router: router,
            isRouteActive: function (route) {
                if (!route.children) {
                    return route.isActive();
                }

                for (var i = 0; i < route.children.length; i++) {
                    var currChild = route.children[i];
                    if (currChild === router.activeItem().title) {
                        return true;
                    }
                }

                return route.isActive();
            }
        };
        
        return shell;

        function activate() {
            return boot();
        }

        function boot() {
            log('FreeDB Loaded!', null, true);

            router.on('router:route:not-found', function (fragment) {
                logError('No Route Found', fragment, true);
            });

            var routes = [
                { route: '', moduleId: 'home', title: 'Discs', nav: 1, children: ['Disc'] },
                { route: 'Discs/:id', moduleId: 'disc', title: 'Disc' },
                { route: 'Artists', moduleId: 'artists', title: 'Artists', nav: 2, children: ['Artist'] },
                { route: 'Artists/:id', moduleId: 'artist', title: 'Artist' }];
            

            return router.makeRelative({ moduleId: 'viewmodels' }) 
                .map(routes)
                .buildNavigationModel()
                .activate();
        }

        function log(msg, data, showToast) {
            logger.log(msg, data, system.getModuleId(shell), showToast);
        }

        function logError(msg, data, showToast) {
            logger.logError(msg, data, system.getModuleId(shell), showToast);
        }
    });