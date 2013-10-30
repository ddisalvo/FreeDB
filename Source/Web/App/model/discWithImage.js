define(function () {
    var discWithImage = function (data) {
        var self = this;

        ko.mapping.fromJS(data, {}, self);

        self.CoverImage = ko.onDemandObservable(function() {
            var request = 'https://itunes.apple.com/search?entity=album&term=' + escape(self.ArtistName()) + ' ' + escape(self.Title());
            $.ajax({
                async: false,
                url: request,
                dataType: 'jsonp'
            }).done(function(result) {
                if (result.resultCount > 0) {
                    self.CoverImage(result.results[0].artworkUrl100);
                } else {
                    self.CoverImage('../content/images/not_available.jpg');
                }
            });
        }, self);
    };

    return discWithImage;
});