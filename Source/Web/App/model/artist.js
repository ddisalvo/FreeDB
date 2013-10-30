define(function (require) {
    var disc = require('model/discWithImage');

    var artist = function (data) {
        var self = this;

        ko.mapping.fromJS(data, {}, self);
        ko.mapping.fromJS(data.Discs.Items, {
            create: function (options) {
                return new disc(options.data);
            }
        }, self.Discs.Items);
    };

    return artist;
});